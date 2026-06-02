import { Component } from '@angular/core';
import { HeaderHome } from '../../../shared/header-home/header-home';
import { FooterHome } from '../../../shared/footer-home/footer-home';
import { RouterOutlet } from '@angular/router';


@Component({
  selector: 'app-publish-layout',
  standalone: true,
  imports: [RouterOutlet, HeaderHome,FooterHome],
  templateUrl: './publish-layout.component.html',
  styleUrl: './publish-layout.component.scss',
})
export class PublishLayoutComponent {

}
