import { Component, computed, inject, OnInit, signal } from '@angular/core';
import { CategoryService } from '../../../../core/services/admin/category';
import { CategoryDto, FlatOption, UpdateCategoryDto } from '../../../../core/models/admin/category.model';
import { ToastService } from '../../../../shared/toast/toast.service';
import { RouterLink } from '@angular/router';
import { CommonModule, NgTemplateOutlet } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { TranslateModule, TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'app-category',
  imports: [NgTemplateOutlet,CommonModule,RouterLink,ReactiveFormsModule,TranslateModule],
  templateUrl: './category.component.html',
  styleUrl: './category.component.scss',
})
export class CategoryComponent implements OnInit{
  private cateService=inject(CategoryService);
  private toast=inject(ToastService);
  private fb=inject(FormBuilder);
  private translate = inject(TranslateService);

  loading=signal(true);
  openSet=signal<Set<string>>(new Set());
  category=signal<CategoryDto[]>([]);

  get rootCategoryCount(): number {
    return this.category().length;
  }

  get totalCategoryCount(): number {
    const walk = (nodes: CategoryDto[]): number => {
      let total = 0;

      for (const node of nodes) {
        total += 1;
        if (node.children?.length) {
          total += walk(node.children);
        }
      }

      return total;
    };

    return walk(this.category());
  }

  editOpen=signal(false);
  editingId=signal<string | null>(null);
  saving=signal(false);

  editForm=this.fb.nonNullable.group({
    name:this.fb.nonNullable.control('',[Validators.required,Validators.maxLength(100)]),
    parentId:this.fb.control<string|null>(null)

  });

  deleteOpen=signal(false);
  deletingId=signal<string | null>(null);
  deletingName=signal<string>('');
  deletingBusy=signal(false);

  parentOptions=computed<FlatOption[]>(()=>{
    const out:FlatOption[]=[];
    const walk=(nodes:CategoryDto[],level:number)=>{
      for (const n of nodes){
        out.push({id:n.id,label:n.name,level})
        if(n.children && n.children.length>0){
          walk(n.children,level+1);
        }
      }
    };
    walk(this.category(),0);
    return out;
  })

  ngOnInit(): void {
    this.loadData();
  }
  loadData(){
    this.loading.set(true);
    this.cateService.getAllCategories().subscribe({
      next:(data)=>{
        this.category.set(data);
        this.loading.set(false);
      },error:(err)=>{
        this.loading.set(false);
        // Giữ fallback để UI vẫn có thông báo dù response chưa được map code.
        this.toast.show(err?.error?.message || this.translate.instant('ui.loadCategoriesFailed'),'error');
      }
    })
  }
  isOpen(id:string){
    return this.openSet().has(id);
  }
  toogle(id:string){
    const next=new Set(this.openSet());
    next.has(id)? next.delete(id) : next.add(id);
    this.openSet.set(next);
  }

  openEdit(node:CategoryDto){
    console.log('node.parentId', node.parentId);
console.log('options contains?', this.parentOptions().some(o => o.id === node.parentId));

    this.editingId.set(node.id);
    this.editForm.reset({
      name:node.name,
      parentId:node.parentId ?? null
    });
    this.editOpen.set(true);
  }
  closeEdit(){
    this.editOpen.set(false);
    this.editingId.set(null);
    this.editForm.reset({name:'',parentId:null});
  }
  isParentOptionDisables(optId:string){
    return this.editingId()===optId;
  }
  saveEdit(){
    if(this.editForm.invalid){
      this.editForm.markAllAsTouched();
      return;
    }
    const id=this.editingId();
    if(!id){
      return;
    }
    this.saving.set(true);
    const dto:UpdateCategoryDto={
      name:this.editForm.controls.name.value,
      parentId:this.editForm.controls.parentId.value ?? undefined
    }
    this.cateService.updateCategory(id,dto).subscribe({
      next:()=>{
        this.saving.set(false);
        this.toast.show(this.translate.instant('ui.categoryUpdated'),'success');
        this.closeEdit();
        this.loadData();
      },error:(err)=>{
        this.saving.set(false);
        this.toast.show(err?.error?.message || this.translate.instant('ui.updateCategoryFailed'),'error');
      }
    })
  }

  //
  openDelete(node:CategoryDto){
    this.deletingId.set(node.id);
    this.deletingName.set(node.name);
    this.deleteOpen.set(true);
  }
  closeDelete(){
    this.deletingId.set(null);
    this.deletingName.set('');
    this.deleteOpen.set(false);
  }
  confirmDelete(){
    const id=this.deletingId();
    if(!id){
      return;
    }
    this.deletingBusy.set(true);
    this.cateService.deleteCategory(id).subscribe({
      next:()=>{
        this.deletingBusy.set(false);
        this.toast.show(this.translate.instant('ui.categoryDeleted'),'success');
        this.closeDelete();
        this.loadData();
      },error:(err)=>{
        this.deletingBusy.set(false);
        this.toast.show(err?.error?.message || this.translate.instant('ui.deleteCategoryFailed'),'error');
      }
    });
  }
}
