import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { DeploymentService, Deployment } from './deployment.service';
import { environment } from '../../environments/environment';

describe('DeploymentService', () => {
  let service: DeploymentService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [DeploymentService]
    });
    service = TestBed.inject(DeploymentService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should get deployments with limit', () => {
    const mockDeployments: Deployment[] = [
      {
        id: 1,
        environment: 'Production',
        version: 'v1.0.0',
        status: 'Success',
        deployedBy: 'Admin',
        startedAt: new Date()
      }
    ];

    service.getDeployments(10).subscribe(deployments => {
      expect(deployments).toEqual(mockDeployments);
    });

    const req = httpMock.expectOne(`${environment.apiUrl}/deployments?limit=10`);
    expect(req.request.method).toBe('GET');
    req.flush(mockDeployments);
  });

  it('should create a deployment', () => {
    const newDeployment: Partial<Deployment> = {
      environment: 'Test',
      version: 'v2.0.0',
      deployedBy: 'User'
    };

    const createdDeployment: Deployment = {
      id: 1,
      environment: 'Test',
      version: 'v2.0.0',
      status: 'Pending',
      deployedBy: 'User',
      startedAt: new Date()
    };

    service.createDeployment(newDeployment).subscribe(deployment => {
      expect(deployment).toEqual(createdDeployment);
    });

    const req = httpMock.expectOne(`${environment.apiUrl}/deployments`);
    expect(req.request.method).toBe('POST');
    req.flush(createdDeployment);
  });

  it('should get deployment stats', () => {
    const mockStats = {
      Total: 100,
      Successful: 85,
      Failed: 10,
      InProgress: 5,
      SuccessRate: 85.0
    };

    service.getStats().subscribe(stats => {
      expect(stats).toEqual(mockStats);
    });

    const req = httpMock.expectOne(`${environment.apiUrl}/deployments/stats`);
    expect(req.request.method).toBe('GET');
    req.flush(mockStats);
  });
});
