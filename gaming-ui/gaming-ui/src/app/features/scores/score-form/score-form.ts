// score-form.ts

import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import {
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators
} from '@angular/forms';

import { ScoreService } from '../../../services/score';
import { Score } from '../../../models/score.model';

@Component({
  selector: 'app-score-form',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule
  ],
  templateUrl: './score-form.html',
  styleUrls: ['./score-form.css']
})
export class ScoreFormComponent implements OnInit {

  scoreForm: FormGroup;

  loading = false;

  successMessage = '';

  scores: Score[] = [];

  editMode = false;

  selectedId = 0;

  constructor(
    private fb: FormBuilder,
    private scoreService: ScoreService
  ) {

    this.scoreForm = this.fb.group({
      playerId: ['', Validators.required],
      gameId: ['', Validators.required],
      points: ['', Validators.required]
    });
  }

  ngOnInit(): void {
    this.loadScores();
  }

  // ================= LOAD SCORES =================

  loadScores(): void {

    this.scoreService.getScores()
      .subscribe({
        next: (data) => {
          this.scores = data;
        },

        error: (err) => {
          console.error(err);
        }
      });
  }

  // ================= SUBMIT =================

  submit(): void {

    if (this.scoreForm.invalid) {
      this.scoreForm.markAllAsTouched();
      return;
    }

    this.loading = true;

    const score: Score = {
      id: this.selectedId,
      playerId: Number(this.scoreForm.value.playerId),
      gameId: Number(this.scoreForm.value.gameId),
      points: Number(this.scoreForm.value.points),
      createdAt: new Date()
    };

    // ================= CREATE =================

    if (!this.editMode) {

      this.scoreService.createScore(score)
        .subscribe({
          next: () => {

            this.successMessage = 'Score created successfully';

            this.scoreForm.reset();

            this.loading = false;

            this.loadScores();
          },

          error: (err: any) => {
            console.error(err);
            this.loading = false;
          }
        });
    }

    // ================= UPDATE =================

    else {

      this.scoreService.updateScore(this.selectedId, score)
        .subscribe({
          next: () => {

            this.successMessage = 'Score updated successfully';

            this.scoreForm.reset();

            this.loading = false;

            this.editMode = false;

            this.selectedId = 0;

            this.loadScores();
          },

          error: (err: any) => {
            console.error(err);
            this.loading = false;
          }
        });
    }
  }

  // ================= EDIT =================

  editScore(score: Score): void {

    this.editMode = true;

    this.selectedId = score.id;

    this.scoreForm.patchValue({
      playerId: score.playerId,
      gameId: score.gameId,
      points: score.points
    });
  }

  // ================= DELETE =================

  deleteScore(id: number): void {

    if (!confirm('Delete this score?')) {
      return;
    }

    this.scoreService.deleteScore(id)
      .subscribe({
        next: () => {

          this.successMessage = 'Score deleted successfully';

          this.loadScores();
        },

        error: (err) => {
          console.error(err);
        }
      });
  }
}