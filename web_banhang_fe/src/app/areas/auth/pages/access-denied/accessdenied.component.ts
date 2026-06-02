import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';
import { TranslateModule } from '@ngx-translate/core';

@Component({
  selector: 'app-access-denied',
  imports: [RouterLink, TranslateModule],
  templateUrl: './accessdenied.component.html',
  styleUrl: './accessdenied.component.scss',
})
export class AccessDeniedComponent {

}
