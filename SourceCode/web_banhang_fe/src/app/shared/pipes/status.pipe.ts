import { Pipe, PipeTransform, inject } from "@angular/core";
import { TranslateService } from '@ngx-translate/core';

@Pipe({
    name: 'status',
    standalone: true,
    pure: false

})
export class StatusPipe implements PipeTransform {
    private translate = inject(TranslateService);

    transform(value: any,):string {
        if(typeof value === 'boolean'){
            return value ? this.translate.instant('ui.status.approved') : this.translate.instant('ui.status.pending');
        }
        if (typeof value === 'string') {
            const normalized = value.trim().toLowerCase();
            if (normalized === 'pending' || normalized === 'approved' || normalized === 'rejected' || normalized === 'unknown') {
                return this.translate.instant(`ui.status.${normalized}`);
            }
            return value;
        }

        switch (value) {
            case 0:
                return this.translate.instant('ui.status.pending');
            case 1:
                return this.translate.instant('ui.status.approved');
            case 2:
                return this.translate.instant('ui.status.rejected');
            default:
                return this.translate.instant('ui.status.unknown');
        }
    }
}