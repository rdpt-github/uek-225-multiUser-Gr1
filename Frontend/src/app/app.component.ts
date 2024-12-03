import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { LoginComponent } from "./login/login.component";
import { LedgerComponent } from './ledger/ledger.component';
import {GambleComponent} from './gamble/gamble.component';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, GambleComponent],
  templateUrl: './app.component.html',
  styleUrl: './app.component.sass',
})
export class AppComponent {
  title = 'L-Bank';
}
