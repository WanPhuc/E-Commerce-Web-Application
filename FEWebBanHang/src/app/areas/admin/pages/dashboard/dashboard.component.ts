import { BaseChartDirective, NG_CHARTS_CONFIGURATION } from 'ng2-charts';
import { Toast } from './../../../../shared/toast/toast.component';
import { Component, inject, OnInit, signal } from '@angular/core';
import { DashboardService } from '../../../../core/services/admin/dashboard';
import { ToastService } from '../../../../shared/toast/toast.service';
import { DashboardChartPointDto, DashboardDto, DashboardMetric, DashboardRanger } from '../../../../core/models/admin/dashboard.model';
import { CommonModule } from '@angular/common';
import { Chart, ChartConfiguration, ChartData } from 'chart.js';
import { provideCharts,withDefaultRegisterables } from 'ng2-charts';


@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule,BaseChartDirective],
  providers:[provideCharts(withDefaultRegisterables())],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.scss',
})
export class DashboardComponent implements OnInit{
  private dashboardService = inject(DashboardService);
  private toast= inject(ToastService);

  loading=signal(true);
  range = signal<DashboardRanger>('Week');
  selectMetric = signal<DashboardMetric>('All');
  chartData=signal<ChartData<'line'>>({labels:[],datasets:[]});
  private pointsCache:DashboardChartPointDto[]=[];
  dashboardStats=signal<DashboardDto|null>(null);

  chartOptions:ChartConfiguration<'line'>['options']={
    responsive:true,
    interaction:{mode:'index',intersect:false},
    maintainAspectRatio: false,
    scales:{
      y:{type:'linear',position:'left',title:{display:true,text:'Count'},beginAtZero:true},
      y1:{type:'linear',position:'right',title:{display:true,text:'Revenue'},grid:{drawOnChartArea:false},beginAtZero:true,ticks:{callback:(value)=>Number(value).toLocaleString('vi-VN')+'đ'}}
    }
  }

  ngOnInit(): void {
    this.loadDashboardStats();
    
  }
  loadDashboardStats(){
    this.loading.set(true);
    this.dashboardService.getDashboardStats().subscribe({
      next:(stats)=>{
        this.loading.set(false);
        this.dashboardStats.set(stats);
        this.loadChartData();
        console.log(stats);
      },
      error:(err)=>{
        this.loading.set(false);
        this.toast.show(err?.error?.message||'Lỗi khi tải thống kê dashboard','error' );
      }
    })
  }
  changeRange(r:DashboardRanger){
    this.range.set(r);
    this.loadChartData();
  }
  changeMetric(m:DashboardMetric){
    this.selectMetric.set(m);
    this.buildChartData(this.pointsCache);
  }
  loadChartData(){
    this.dashboardService.getChart(this.range()).subscribe({
      next:(chart)=>{
        this.pointsCache=chart.points ??[];
        this.buildChartData(chart.points);
      },
      error:(err)=>{
        this.toast.show(err?.error?.message||'Lỗi khi tải dữ liệu biểu đồ','error' );
      }
    })
  }
  buildChartData(pts:DashboardChartPointDto[]){
    const sel =this.selectMetric();
    this.chartData.set({
      labels:pts.map(p=>p.label),
      datasets:[
        {label:'Revenue',data:pts.map(x=>x.revenue),yAxisID:'y1',hidden:sel!=='All' && sel!=='Revenue',borderColor:'blue',backgroundColor:'blue'},
        {label:'Orders',data:pts.map(x=>x.orders),yAxisID:'y',hidden:sel!=='All' && sel!=='Orders',borderColor:'green',backgroundColor:'green'},    
        {label:'Users',data:pts.map(x=>x.users),yAxisID:'y',hidden:sel!=='All' && sel!=='Users',borderColor:'orange',backgroundColor:'orange'},
        {label:'Sellers',data:pts.map(x=>x.sellers),yAxisID:'y',hidden:sel!=='All' && sel!=='Sellers',borderColor:'purple',backgroundColor:'purple'},
      ]
    })
  }
}
