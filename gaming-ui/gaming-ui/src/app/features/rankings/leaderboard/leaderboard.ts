// leaderboard.ts

import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import {
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators
} from '@angular/forms';

import { RankingService } from '../../../services/ranking';
import { Ranking } from '../../../models/ranking.model';

@Component({
  selector: 'app-leaderboard',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule
  ],
  templateUrl: './leaderboard.html',
  styleUrls: ['./leaderboard.css']
})
export class LeaderboardComponent {

  rankings: Ranking[] = [];

  loading = false;

  searchForm: FormGroup;

  constructor(
    private fb: FormBuilder,
    private rankingService: RankingService
  ) {

    this.searchForm = this.fb.group({
      gameId: ['', Validators.required]
    });
  }

  search(): void {

    if (this.searchForm.invalid) {
      this.searchForm.markAllAsTouched();
      return;
    }

    this.loading = true;

    const gameId = Number(
      this.searchForm.value.gameId
    );

    this.rankingService
      .getRankings(gameId)
      .subscribe({
        next: (data: Ranking[]) => {

          this.rankings = data;

          this.loading = false;
        },

        error: (err: any) => {

          console.error(err);

          this.loading = false;
        }
      });
  }
}