// services/ranking.ts

import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { Ranking } from '../models/ranking.model';

@Injectable({
  providedIn: 'root',
})
export class RankingService {

  private apiUrl = 'https://localhost:56850/rankings';

  constructor(private http: HttpClient) {}

  // ================= GET RANKINGS BY GAME =================

  getRankings(gameId: number): Observable<Ranking[]> {
    return this.http.get<Ranking[]>(
      `${this.apiUrl}/${gameId}`
    );
  }
}