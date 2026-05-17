// game-list.ts

import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';

import { GameService } from '../../../services/game';
import { Game } from '../../../models/game.model';

@Component({
  selector: 'app-game-list',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './game-list.html',
  styleUrls: ['./game-list.css'],
})
export class GameListComponent implements OnInit {

  games: Game[] = [];

  loading = false;

  constructor(private gameService: GameService) {}

  ngOnInit(): void {
    this.loadGames();
  }

  loadGames(): void {

    this.loading = true;

    this.gameService.getGames().subscribe({

      next: (data: Game[]) => {
        this.games = data;
        this.loading = false;
      },

      error: (err: any) => {
        console.error(err);
        this.loading = false;
      }

    });
  }
}