import { SellerDashboardChartPointDto, SellerDashboardDto } from './../../../../core/models/seller/sellerDashboard.model';
import { StatusOrder } from '../../../../shared/types/enum/StatusOrder';
import { ChartConfiguration, ChartData } from 'chart.js';
import { Component, inject, OnInit, signal } from '@angular/core';
import { ChartRanger } from '../../../../shared/types/chartRanger';
import { SellerDashboardMetric } from '../../../../core/models/seller/sellerDashboard.model';
import { SellerDashboardService } from '../../../../core/services/seller/sellerDashboardService';
import { ToastService } from '../../../../shared/toast/toast.service';
import { CommonModule } from '@angular/common';
import { BaseChartDirective, provideCharts, withDefaultRegisterables } from "ng2-charts";
import { TimeAgoPipe } from '../../../../shared/pipes/timeAgoPipe.pipe';
import { PaymentStatus } from '../../../../shared/types/enum/PaymentStatus';
import { TranslateModule, TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'app-dashboard',
  imports: [CommonModule, BaseChartDirective,TimeAgoPipe, TranslateModule],
  providers:[provideCharts(withDefaultRegisterables())],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.scss',
})
export class SellerDashboardComponent implements OnInit{
  private dashService = inject(SellerDashboardService);
  private toast = inject(ToastService);
  private translate = inject(TranslateService);

  loading  =signal(false);
  ranger=signal<ChartRanger>('Week');
  selectMetric=signal<SellerDashboardMetric>('All');
  chartData=signal<ChartData<'line'>>({labels:[],datasets:[]});
  pointCache : SellerDashboardChartPointDto[]=[];
  dashboardStats=signal<SellerDashboardDto|null>(null);

  chartOptions:ChartConfiguration<'line'>['options']={
    responsive:true,
    interaction:{mode:'index',intersect:false},
    maintainAspectRatio:false,
    scales:{
        x: {
        display: true,
        type: 'category',   // ← quan trọng: fix label không hiện
        title: { display: true, text: this.translate.instant('ui.date') },
        ticks: {
          maxRotation: 45,
          autoSkip: true,   // ← tránh label bị chồng chéo
          maxTicksLimit: 10,
        },
      },
      y:{type:'linear',position:'left',title:{display:true,text:this.translate.instant('ui.count')},beginAtZero:true},
      y1:{type:"linear",position:'right',title:{display:true,text:this.translate.instant('ui.revenue')},beginAtZero:true,grid:{drawOnChartArea:false},ticks:{callback:(value)=>Number(value).toLocaleString('vi-VN')+'đ'}}
    }
  }
  ngOnInit(): void {
    this.loadDashboardStats();
  }
  loadDashboardStats(){
    this.loading.set(true);
    this.dashService.getDashboardStats().subscribe({
      next:(stats)=>{
        this.loading.set(false);
        this.dashboardStats.set(stats);
        this.loadChartData();
        console.log(stats);
      },
      error:(err)=>{
        this.loading.set(false);
        this.toast.show(err?.error?.message||this.translate.instant('ui.loadDashboardFailed'),'error' );
      }
    });
  }
  changeRanger(r:ChartRanger){
    this.ranger.set(r);
    this.loadChartData();
  }
  changeMetric(m:SellerDashboardMetric){
    this.selectMetric.set(m);
    this.buidChartData(this.pointCache);
  }
  buidChartData(pts:SellerDashboardChartPointDto[]){
    const sel = this.selectMetric();
    this.chartData.set({
      labels:pts.map(p=>p.label),
      datasets:[
        {label:this.translate.instant('ui.revenue'),data:pts.map(p=>p.revenue),yAxisID:'y1',hidden:sel!=='All'&&sel!=='Revenue',borderColor:'yellow',backgroundColor:'rgba(255, 255, 0, 0.3)',tension:0.4,fill:true,spanGaps:true},
        {label:this.translate.instant('ui.orders'),data:pts.map(p=>p.orders),yAxisID:'y',hidden:sel!=='All'&&sel!=='Orders',borderColor:'blue',backgroundColor:'rgba(0, 0, 255, 0.3)',tension:0.4,fill:true,spanGaps:true},
      ]
    });
    
  }

  loadChartData(){
    this.dashService.getChart(this.ranger()).subscribe({
      next:(chart)=>{
        this.pointCache=chart.points??[];
        this.buidChartData(chart.points);
      },
      error:(err)=>{
        this.toast.show(err?.error?.message||this.translate.instant('ui.loadChartFailed'),'error' );
      }
    })
  }

  getStatusDetails(status: StatusOrder | string){
    const value = typeof status === 'string' ? status : StatusOrder[status];

    switch(value){
      case StatusOrder[StatusOrder.Pending]:
        return {text: this.translate.instant('ui.orderStatus.pending'), class: 'bg-warning-subtle text-warning'};
      case StatusOrder[StatusOrder.Paid]:
        return {text: this.translate.instant('ui.orderStatus.paid'), class: 'bg-success-subtle text-success'};
      case StatusOrder[StatusOrder.Processing]:
        return {text: this.translate.instant('ui.orderStatus.processing'), class: 'bg-primary-subtle text-primary'};
      default:
        return {text: value ?? this.translate.instant('ui.unknown'), class: 'bg-secondary-subtle text-secondary'};
    }
  }
  getPaymentStatusDetails(status: number | string){
    const value= typeof status ==='string'? status:PaymentStatus[status];
    switch(value){
      case PaymentStatus[PaymentStatus.Pending]:
        return {text: this.translate.instant('ui.paymentStatus.pending'), class: 'bg-warning-subtle text-warning'};
      case PaymentStatus[PaymentStatus.Paid]:
        return {text: this.translate.instant('ui.paymentStatus.paid'), class: 'bg-success-subtle text-success'};
      default:
        return {text: value ?? this.translate.instant('ui.unknown'), class: 'bg-secondary-subtle text-secondary'};
    }
  }

}
