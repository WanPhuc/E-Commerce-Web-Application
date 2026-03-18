import { CreateCategoryDto } from './../../../../../core/models/admin/category.model';
import { Component, computed, inject, OnInit, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { ToastService } from '../../../../../shared/toast/toast.service';
import { CategoryService } from '../../../../../core/services/admin/category';
import { CategoryDto, FlatOption } from '../../../../../core/models/admin/category.model';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-create',
  imports: [CommonModule, RouterLink,ReactiveFormsModule],
  templateUrl: './create.component.html',
  styleUrl: './create.component.scss',
})
export class CreateComponent implements OnInit{
  private fb=inject(FormBuilder);
  private route=inject(ActivatedRoute);
  private router=inject(Router);
  private toast=inject(ToastService);
  private cateService=inject(CategoryService);

  loading=signal(true);
  submiting=signal(false);

  category=signal<CategoryDto[]>([]);

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
  });

  form=this.fb.group({
    name:this.fb.nonNullable.control('',[Validators.required,Validators.maxLength(100)]),
    parentId:this.fb.nonNullable.control<string | null>(null),
  })

  ngOnInit(): void {
    const parentIdFormQuery=this.route.snapshot.queryParamMap.get('parentId');
    if(parentIdFormQuery){this.form.patchValue({parentId:parentIdFormQuery})}
    this.loadCategories();
  }
  loadCategories(){
    this.loading.set(true);
    this.cateService.getAllCategories().subscribe({
      next:(data)=>{
        this.category.set(data);
        this.loading.set(false);

        const currenParentId=this.form.value.parentId;
        if(currenParentId && !this.parentOptions().some(o=>o.id===currenParentId)){
          this.form.patchValue({parentId:null});
        }
      },error:(err)=>{
        this.loading.set(false);
        this.toast.show(err?.error?.message || 'Load categories failed','error');
      }
    })
  }

  submit(){
    if(this.form.invalid){
      this.form.markAllAsTouched();
      return;
    }
    const dto:CreateCategoryDto={
      name:this.form.value.name!.trim(),
      parentId:this.form.value.parentId || undefined,
    }
    this.submiting.set(true);
    this.cateService.createCategory(dto).subscribe({
      next:()=>{
        this.submiting.set(false);
        this.toast.show('Create category successfully','success');
        this.router.navigate(['/admin/category']);
      },error:(err)=>{
        this.submiting.set(false);
        this.toast.show(err?.error?.message || 'Create category failed','error');
      }
    })
  }

}
