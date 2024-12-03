import {Component, inject, input, OnInit} from '@angular/core';
import {FormsModule} from '@angular/forms';
import {GambleService} from '../../services/gamble.service';
import {Ledger} from '../../models/ledger.model';
import {NgForOf} from '@angular/common';
import {LedgerService} from '../../services/ledger.service';
import {GambleResultModel} from '../../models/gambleResult.model';
import {tap} from 'rxjs';

declare function rollDice(arg: any): void;

@Component({
  selector: 'app-gamble',
  templateUrl: './gamble.component.html',
  styleUrl: './gamble.component.css',
  standalone: true,
  imports: [
    FormsModule,
    NgForOf
  ],
})
export class GambleComponent implements OnInit {
  ledgerService = inject(LedgerService);
  ledgers: Ledger[] = [];
  numbers!: number[];
  id: number | null = null;
  result: GambleResultModel = {
    winAmount: 69,
    correctNumbers: 2
  }

  constructor(private gambleService: GambleService) {
  }

  ngOnInit(): void {
    this.loadLedgers();
  }

  loadLedgers(): void {
    this.ledgerService.getLedgers().subscribe(
      {
        error: (error) => console.error('Error fetching ledgers', error),
        next: (data: Ledger[]) => this.ledgers = data
      }
    );
  }

  roll3Dice() {
    this.numbers = [];
    for (let i = 0; i < 3; i++) {
      const randomNumbers = Math.floor(Math.random() * 6) + 1;
      this.numbers.push(randomNumbers);
    }
    console.log(this.numbers);

    this.gambleService.gamble(this.id!, this.numbers).pipe(tap((data: GambleResultModel) => {
      this.result = data
    })).subscribe();

    console.log(this.result);
    rollDice(this.numbers);
  }
}
