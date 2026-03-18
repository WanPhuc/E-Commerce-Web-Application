import { HttpInterceptorFn } from '@angular/common/http';

export const credentialsInterceptor: HttpInterceptorFn = (req, next) => {
  // Clone request và thêm withCredentials cho TẤT CẢ requests
  const clonedReq = req.clone({
    withCredentials: true
  });

  return next(clonedReq);
};