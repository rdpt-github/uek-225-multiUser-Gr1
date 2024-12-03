import {Component, inject, input} from '@angular/core';
import {FormsModule} from '@angular/forms';
import {GambleService} from '../../services/gamble.service';
import {Ledger} from '../../models/ledger.model';
import {NgForOf} from '@angular/common';
import {LedgerService} from '../../services/ledger.service';
import {GambleResultModel} from '../../models/gambleResult.model';

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
export class GambleComponent {
  ledgerService = inject(LedgerService);
  ledgers: Ledger[] = [];
  numbers!: number[];
  id : number | null = null;
  result!: GambleResultModel;
  constructor(private gambleService: GambleService) {
  }

  ngOnInit(): void {
    this.loadLedgers();
  }

  loadLedgers(): void {
    this.ledgerService.getLedgers().subscribe(
      (data) => (this.ledgers = data),
      (error) => console.error('Error fetching ledgers', error)
    );
  }

  roll3Dice(){
    this.numbers = [];
    for (let i=0; i<3; i++) {
      const randomNumbers = Math.floor(Math.random() * 6) + 1;
      this.numbers.push(randomNumbers);
    }

    rollDice(this.numbers);
    this.gambleService.gamble(this.id!, this.numbers).subscribe({
      next: (response: any) => {
        this.result = response;
      }
    })


  }


}
