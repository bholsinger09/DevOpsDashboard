import { ComponentFixture, TestBed } from '@angular/core/testing';
import { IonicModule } from '@ionic/angular';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { DashboardPage } from './dashboard.page';
import { ServerService } from '../../services/server.service';
import { GithubService } from '../../services/github.service';
import { DeploymentService } from '../../services/deployment.service';
import { LogService } from '../../services/log.service';
import { SignalRService } from '../../services/signalr.service';
import { of } from 'rxjs';

describe('DashboardPage', () => {
  let component: DashboardPage;
  let fixture: ComponentFixture<DashboardPage>;
  let serverService: jasmine.SpyObj<ServerService>;
  let githubService: jasmine.SpyObj<GithubService>;
  let deploymentService: jasmine.SpyObj<DeploymentService>;
  let logService: jasmine.SpyObj<LogService>;
  let signalRService: jasmine.SpyObj<SignalRService>;

  beforeEach(async () => {
    const serverServiceSpy = jasmine.createSpyObj('ServerService', ['getStats']);
    const githubServiceSpy = jasmine.createSpyObj('GithubService', ['getStats']);
    const deploymentServiceSpy = jasmine.createSpyObj('DeploymentService', ['getStats', 'getDeployments']);
    const logServiceSpy = jasmine.createSpyObj('LogService', ['getStats', 'getLogs']);
    const signalRServiceSpy = jasmine.createSpyObj('SignalRService', [], {
      serverStatusUpdate$: of(null),
      deploymentNotification$: of(null)
    });

    await TestBed.configureTestingModule({
      declarations: [DashboardPage],
      imports: [IonicModule.forRoot(), HttpClientTestingModule],
      providers: [
        { provide: ServerService, useValue: serverServiceSpy },
        { provide: GithubService, useValue: githubServiceSpy },
        { provide: DeploymentService, useValue: deploymentServiceSpy },
        { provide: LogService, useValue: logServiceSpy },
        { provide: SignalRService, useValue: signalRServiceSpy }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(DashboardPage);
    component = fixture.componentInstance;
    serverService = TestBed.inject(ServerService) as jasmine.SpyObj<ServerService>;
    githubService = TestBed.inject(GithubService) as jasmine.SpyObj<GithubService>;
    deploymentService = TestBed.inject(DeploymentService) as jasmine.SpyObj<DeploymentService>;
    logService = TestBed.inject(LogService) as jasmine.SpyObj<LogService>;
    signalRService = TestBed.inject(SignalRService) as jasmine.SpyObj<SignalRService>;
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should load stats on init', () => {
    const mockServerStats = { total: 10, online: 8, offline: 2, degraded: 0 };
    const mockGithubStats = { issues: { open: 5 }, pullRequests: { open: 3 } };
    const mockDeploymentStats = { total: 50, successful: 45, failed: 5 };
    const mockLogStats = { total: 1000, error: 10, warning: 50 };

    serverService.getStats.and.returnValue(of(mockServerStats));
    githubService.getStats.and.returnValue(of(mockGithubStats));
    deploymentService.getStats.and.returnValue(of(mockDeploymentStats));
    deploymentService.getDeployments.and.returnValue(of([]));
    logService.getStats.and.returnValue(of(mockLogStats));
    logService.getLogs.and.returnValue(of([]));

    component.ngOnInit();

    expect(serverService.getStats).toHaveBeenCalled();
    expect(githubService.getStats).toHaveBeenCalled();
    expect(deploymentService.getStats).toHaveBeenCalled();
    expect(logService.getStats).toHaveBeenCalled();
  });

  it('should return correct status color', () => {
    expect(component.getStatusColor('Online')).toBe('success');
    expect(component.getStatusColor('Offline')).toBe('danger');
    expect(component.getStatusColor('Degraded')).toBe('warning');
  });

  it('should return correct log level color', () => {
    expect(component.getLogLevelColor('Error')).toBe('danger');
    expect(component.getLogLevelColor('Warning')).toBe('warning');
    expect(component.getLogLevelColor('Info')).toBe('primary');
  });
});
