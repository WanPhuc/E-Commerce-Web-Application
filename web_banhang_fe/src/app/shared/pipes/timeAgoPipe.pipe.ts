import { Pipe, PipeTransform, inject } from "@angular/core";
import { TranslateService } from '@ngx-translate/core';

@Pipe({
    name: 'timeAgo'
    ,standalone:true,
    pure: false

})
export class TimeAgoPipe implements PipeTransform{
    private translate = inject(TranslateService);

    transform(value: any):string {
        if(!value) return '';
        const date= new Date(value);
        const now = new Date();
        const seconds= Math.floor((now.getTime()-date.getTime())/1000);
        if(seconds<60) return this.translate.instant('ui.timeAgo.justNow');
        const intervals = [
            { key: 'year', seconds: 31536000 },
            { key: 'month', seconds: 2592000 },
            { key: 'day', seconds: 86400 },
            { key: 'hour', seconds: 3600 },
            { key: 'minute', seconds: 60 },
        ];
        for(const interval of intervals){
            const counter = Math.floor(seconds / interval.seconds);
            if(counter>0) return this.translate.instant(`ui.timeAgo.${interval.key}`, { count: counter }); 
        }
        return this.translate.instant('ui.timeAgo.justNow');
    
    }
}