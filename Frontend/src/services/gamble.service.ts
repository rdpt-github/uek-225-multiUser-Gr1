import {Injectable} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {AuthService} from './auth.service';
import {Observable} from 'rxjs';
import {Ledger} from '../models/ledger.model';
import {GambleResultModel} from '../models/gambleResult.model';

@Injectable({
  providedIn: 'root',
})
export class GambleService {
  private apiUrl = 'http://localhost:5000/api/v1';

  constructor(private http: HttpClient, private authService: AuthService) {}

  gamble(id: number, numbers: number[]): Observable<GambleResultModel> {
    const token = this.authService.getToken();
    if (token) {
      return this.http.get<GambleResultModel>(`${this.apiUrl}/gamble/{id}`);
    }

    return new Observable<GambleResultModel>();
  }
}
