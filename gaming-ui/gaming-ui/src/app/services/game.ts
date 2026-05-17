import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { Game } from '../models/game.model';
import { GameDetails } from '../models/game-details.model';

@Injectable({
  providedIn: 'root',
})
export class GameService {

  // ================= API BASE URL =================

  private apiUrl = 'https://localhost:56623/api/game';

  constructor(private http: HttpClient) {}

  // ================= GET ALL GAMES =================

  getGames(): Observable<Game[]> {

    return this.http.get<Game[]>(
      this.apiUrl
    );
  }

  // ================= GET GAME BY ID =================

  getGame(id: number): Observable<GameDetails> {

    return this.http.get<GameDetails>(
      `${this.apiUrl}/${id}`
    );
  }

  // ================= CREATE GAME =================

  createGame(
    game: Partial<Game>
  ): Observable<Game> {

    return this.http.post<Game>(
      this.apiUrl,
      game
    );
  }
}