import { CommonModule } from '@angular/common';
import { ToastService } from './toast.service';
import { Component } from '@angular/core';

@Component({
  selector: 'app-toast',
  imports: [CommonModule],
  templateUrl: './toast.component.html',
  styleUrl: './toast.component.scss',
})
export class Toast {
  constructor(public toast: ToastService) {}
}
