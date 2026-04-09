import { Pipe, PipeTransform } from "@angular/core";

@Pipe({
    name: 'timeAgo'
    ,standalone:true

})
export class TimeAgoPipe implements PipeTransform{
    transform(value: any):string {
        if(!value) return '';
        const date= new Date(value);
        const now = new Date();
        const seconds= Math.floor((now.getTime()-date.getTime())/1000);
        if(seconds<60) return 'Vừa xong';
        const intervals:{[key:string]:number}={
            'năm':31536000,
            'tháng':2592000,
            'ngày':86400,
            'giờ':3600,
            'phút':60
        }
        for(const key in intervals){
            const counter =Math.floor(seconds/intervals[key]);
            if(counter>0) return `${counter} ${key} trước`; 
        }
        return 'Vừa xong';
    
    }
}