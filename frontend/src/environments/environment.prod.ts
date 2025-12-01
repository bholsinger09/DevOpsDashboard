export const environment = {
  production: true,
  // For Docker deployment with nginx proxy
  apiUrl: 'http://localhost:5000/api',
  hubUrl: 'http://localhost:5000/dashboardHub'
  // For cloud deployment, update these to your actual domain:
  // apiUrl: 'https://your-api-url.com/api',
  // hubUrl: 'https://your-api-url.com/dashboardHub'
};
