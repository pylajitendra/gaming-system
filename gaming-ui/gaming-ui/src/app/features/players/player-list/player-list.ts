// player-list.ts

import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';

import { PlayerService } from '../../../services/player';
import { Player } from '../../../models/player.model';

@Component({
  selector: 'app-player-list',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './player-list.html',
  styleUrls: ['./player-list.css'],
})
export class PlayerListComponent implements OnInit {
  players: Player[] = [];

  loading = false;

  constructor(private playerService: PlayerService) {}

  ngOnInit(): void {
    this.loadPlayers();
  }

  loadPlayers(): void {
    this.loading = true;

    this.playerService.getPlayers().subscribe({
      next: (data: Player[]) => {
        this.players = data;
        this.loading = false;
      },
      error: (err: any) => {
        console.error(err);
        this.loading = false;
      },
    });
  }
}