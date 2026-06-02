import { Injectable, signal } from "@angular/core";

export type ToastType = 'success' | 'error'| 'warning'| 'info';

@Injectable({providedIn: 'root'})
export class ToastService{
    message = signal<string | null>(null);
    type=signal<ToastType>('info');
    show(msg: string, type: ToastType = 'info') {
        this.message.set(msg);
        this.type.set(type);

        setTimeout(() => this.clear(), 3000);
    }

    clear() {
        this.message.set(null);
    }
}