export const environment = {
  apiUrl: process.env.NEXT_PUBLIC_API_URL || 'https://3jk7l7bp-44385.use2.devtunnels.ms',
  production: process.env.NODE_ENV === 'production',
};

export const API_BASE_URL = environment.apiUrl;
