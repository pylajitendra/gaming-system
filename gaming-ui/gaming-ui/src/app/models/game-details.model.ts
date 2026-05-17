// src/app/models/game-details.model.ts

import { Game } from './game.model';
import { Player } from './player.model';

export interface GameDetails {
  game: Game;
  player: Player | string;
}