import { Component, OnInit } from '@angular/core';
import { LedgerService } from '../../services/ledger.service';
import { Ledger } from '../../models/ledger.model';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../services/auth.service';
import { HttpClient, HttpClientModule } from '@angular/common/http';

@Component({
  selector: 'app-ledger',
  templateUrl: './ledger.component.html',
  styleUrls: ['./ledger.component.css'],
  imports: [CommonModule, FormsModule],
  standalone: true,
  providers:  [ LedgerService, HttpClient ]
})
export class LedgerComponent implements OnInit {
  ledgers: Ledger[] = [];
  fromLedgerId: number | null = null;
  toLedgerId: number | null = null;
  amount: number | null = null;
  transferMessage: string = '';
  ledgerName: string = '';
  balance: number | null = null;

  constructor(private ledgerService: LedgerService) {}

  ngOnInit(): void {
    this.loadLedgers();
  }

  loadLedgers(): void {
    this.ledgerService.getLedgers().subscribe(
      (data) => (this.ledgers = data),
      (error) => console.error('Error fetching ledgers', error)
    );
  }

  makeTransfer(): void {
    if (
      this.fromLedgerId !== null &&
      this.toLedgerId !== null &&
      this.amount !== null &&
      this.amount > 0
    ) {
      this.ledgerService
        .transferFunds(this.fromLedgerId, this.toLedgerId, this.amount)
        .subscribe(
          () => {
            this.transferMessage = 'Transfer successful!';
            this.loadLedgers(); // Refresh ledger balances
          },
          (error) => {
            this.transferMessage = `Transfer failed: ${error.error.message}`;
            console.error('Transfer error', error);
          }
        );
    } else {
      this.transferMessage = 'Please fill in all fields with valid data.';
    }
  }

  printMoney(ledgerId : number): void {
    if (ledgerId !== null){
      this.ledgerService.add100ToLedger(ledgerId).subscribe(
        () => {
          this.transferMessage = 'Money printed successfully!';
          this.loadLedgers();
        },
        (error) => {
          this.transferMessage = `Money printing failed: ${error.error.message}`;
          console.error('Money printing error', error);
        }
      );
    }
  }

  createLedger(): void {
    if (this.ledgerName && this.balance !== null) {
      const newLedger: Ledger = {
        id: 0, // The ID will be set by the backend
        name: this.ledgerName,
        balance: this.balance
      };
      this.ledgerService.createLedger(newLedger).subscribe(
        () => {
          this.transferMessage = 'Ledger created successfully!';
          this.loadLedgers();
          this.ledgerName = '';
          this.balance = null;
        },
        (error: { error: { message: any; }; }) => {
          this.transferMessage = `Ledger creation failed: ${error.error.message}`;
          console.error('Ledger creation error', error);
        }
      );
    } else {
      this.transferMessage = 'Please provide a valid name and balance.';
    }
  }
}
