export interface NotificationDto{
    id:string;
    title:string;
    message:string;
    redirectUrl?:string;
    type:number;
    isRead:boolean;
    createdAt:string;
}