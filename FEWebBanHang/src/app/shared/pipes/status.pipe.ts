import { Pipe, PipeTransform } from "@angular/core";
import { pipe } from "rxjs";

@Pipe({
    name: 'status',
    standalone: true

})
export class StatusPipe implements PipeTransform {
    transform(value: any,):string {
        if(typeof value === 'boolean'){
            return value? 'Approved':'Pending';
        }
        if (typeof value === 'string') {
            return value;
        }

        switch (value) {
            case 0:
                return 'Pending';
            case 1:
                return 'Approved';
            case 2:
                return 'Rejected';
            default:
                return 'Unknown';
        }
    }
}