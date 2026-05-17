import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { Score } from '../models/score.model';

@Injectable({
  providedIn: 'root'
})
export class ScoreService {

  private apiUrl = 'https://localhost:62669/scores';

  constructor(private http: HttpClient) { }

  getScores(): Observable<Score[]> {
    return this.http.get<Score[]>(this.apiUrl);
  }

  getScore(id: number): Observable<Score> {
    return this.http.get<Score>(`${this.apiUrl}/${id}`);
  }

  createScore(score: Score): Observable<Score> {
    return this.http.post<Score>(this.apiUrl, score);
  }

  updateScore(id: number, score: Score): Observable<Score> {
    return this.http.put<Score>(`${this.apiUrl}/${id}`, score);
  }

  deleteScore(id: number): Observable<string> {
    return this.http.delete(`${this.apiUrl}/${id}`, {
      responseType: 'text'
    });
  }
}