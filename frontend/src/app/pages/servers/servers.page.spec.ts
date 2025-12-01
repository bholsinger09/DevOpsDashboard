import { ComponentFixture, TestBed } from '@angular/core/testing';
import { IonicModule, AlertController } from '@ionic/angular';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { ServersPage } from './servers.page';
import { ServerService, Server } from '../../services/server.service';
import { SignalRService } from '../../services/signalr.service';
import { of } from 'rxjs';

describe('ServersPage', () => {
  let component: ServersPage;
  let fixture: ComponentFixture<ServersPage>;
  let serverService: jasmine.SpyObj<ServerService>;
  let alertController: jasmine.SpyObj<AlertController>;

  beforeEach(async () => {
    const serverServiceSpy = jasmine.createSpyObj('ServerService', [
      'getServers',
      'createServer',
      'deleteServer'
    ]);
    const signalRServiceSpy = jasmine.createSpyObj('SignalRService', [], {
      serverStatusUpdate$: of(null)
    });
    const alertControllerSpy = jasmine.createSpyObj('AlertController', ['create']);

    await TestBed.configureTestingModule({
      declarations: [ServersPage],
      imports: [IonicModule.forRoot(), HttpClientTestingModule],
      providers: [
        { provide: ServerService, useValue: serverServiceSpy },
        { provide: SignalRService, useValue: signalRServiceSpy },
        { provide: AlertController, useValue: alertControllerSpy }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(ServersPage);
    component = fixture.componentInstance;
    serverService = TestBed.inject(ServerService) as jasmine.SpyObj<ServerService>;
    alertController = TestBed.inject(AlertController) as jasmine.SpyObj<AlertController>;
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should load servers on init', () => {
    const mockServers: Server[] = [
      {
        id: 1,
        name: 'Server 1',
        url: 'https://server1.com',
        status: 'Online',
        uptimePercentage: 99,
        responseTimeMs: 150,
        createdAt: new Date(),
        updatedAt: new Date()
      }
    ];

    serverService.getServers.and.returnValue(of(mockServers));

    component.ngOnInit();

    expect(serverService.getServers).toHaveBeenCalled();
    expect(component.servers.length).toBe(1);
    expect(component.filteredServers.length).toBe(1);
  });

  it('should filter servers by name', () => {
    component.servers = [
      {
        id: 1,
        name: 'Production Server',
        url: 'https://prod.com',
        status: 'Online',
        uptimePercentage: 99,
        responseTimeMs: 150,
        createdAt: new Date(),
        updatedAt: new Date()
      },
      {
        id: 2,
        name: 'Test Server',
        url: 'https://test.com',
        status: 'Online',
        uptimePercentage: 95,
        responseTimeMs: 200,
        createdAt: new Date(),
        updatedAt: new Date()
      }
    ];

    const event = { target: { value: 'Production' } };
    component.filterServers(event);

    expect(component.filteredServers.length).toBe(1);
    expect(component.filteredServers[0].name).toBe('Production Server');
  });

  it('should return correct status color', () => {
    expect(component.getStatusColor('Online')).toBe('success');
    expect(component.getStatusColor('Offline')).toBe('danger');
    expect(component.getStatusColor('Degraded')).toBe('warning');
    expect(component.getStatusColor('Unknown')).toBe('medium');
  });

  it('should return correct status icon', () => {
    expect(component.getStatusIcon('Online')).toBe('checkmark-circle');
    expect(component.getStatusIcon('Offline')).toBe('close-circle');
    expect(component.getStatusIcon('Degraded')).toBe('warning');
    expect(component.getStatusIcon('Unknown')).toBe('help-circle');
  });
});
