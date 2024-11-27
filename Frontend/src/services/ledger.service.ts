import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Ledger } from '../models/ledger.model';
import { AuthService } from './auth.service';

@Injectable({
  providedIn: 'root',
})
export class LedgerService {
  private apiUrl = 'http://localhost:5000/api/v1';

  constructor(private http: HttpClient, private authService: AuthService) {}

  getLedgers(): Observable<Ledger[]> {
    const token = this.authService.getToken();
    if (token) {
      return this.http.get<Ledger[]>(`${this.apiUrl}/ledgers`);
    }

    return new Observable<Ledger[]>();
  }

  transferFunds(
    SourceId: number,
    DestinationId: number,
    Amount: number
  ): Observable<any> {
    const payload = {
      SourceId,
      DestinationId,
      Amount,
    };
    return this.http.post(`${this.apiUrl}/bookings`, payload);
  }

  add100ToLedger(ledgerId: number): Observable<any> {
    return this.http.put(`${this.apiUrl}/MoneyPrinter/${ledgerId}`, null);
  }
}
