import { Routes } from '@angular/router';

import { GameListComponent } from './features/games/game-list/game-list';
import { PlayerListComponent } from './features/players/player-list/player-list';
import { LeaderboardComponent } from './features/rankings/leaderboard/leaderboard';
import { ScoreFormComponent } from './features/scores/score-form/score-form';

export const routes: Routes = [

  {
    path: '',
    redirectTo: 'games',
    pathMatch: 'full'
  },

  {
    path: 'games',
    component: GameListComponent
  },

  {
    path: 'players',
    component: PlayerListComponent
  },

  {
    path: 'rankings',
    component: LeaderboardComponent
  },

  {
    path: 'scores',
    component: ScoreFormComponent
  }

];