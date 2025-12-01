import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { ServerService, Server } from './server.service';
import { environment } from '../../environments/environment';

describe('ServerService', () => {
  let service: ServerService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [ServerService]
    });
    service = TestBed.inject(ServerService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should get all servers', () => {
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
      },
      {
        id: 2,
        name: 'Server 2',
        url: 'https://server2.com',
        status: 'Offline',
        uptimePercentage: 85,
        responseTimeMs: 200,
        createdAt: new Date(),
        updatedAt: new Date()
      }
    ];

    service.getServers().subscribe(servers => {
      expect(servers.length).toBe(2);
      expect(servers).toEqual(mockServers);
    });

    const req = httpMock.expectOne(`${environment.apiUrl}/servers`);
    expect(req.request.method).toBe('GET');
    req.flush(mockServers);
  });

  it('should get a single server by id', () => {
    const mockServer: Server = {
      id: 1,
      name: 'Test Server',
      url: 'https://test.com',
      status: 'Online',
      uptimePercentage: 95,
      responseTimeMs: 100,
      createdAt: new Date(),
      updatedAt: new Date()
    };

    service.getServer(1).subscribe(server => {
      expect(server).toEqual(mockServer);
    });

    const req = httpMock.expectOne(`${environment.apiUrl}/servers/1`);
    expect(req.request.method).toBe('GET');
    req.flush(mockServer);
  });

  it('should create a server', () => {
    const newServer: Partial<Server> = {
      name: 'New Server',
      url: 'https://new.com'
    };

    const createdServer: Server = {
      id: 1,
      name: 'New Server',
      url: 'https://new.com',
      status: 'Unknown',
      uptimePercentage: 0,
      responseTimeMs: 0,
      createdAt: new Date(),
      updatedAt: new Date()
    };

    service.createServer(newServer).subscribe(server => {
      expect(server).toEqual(createdServer);
    });

    const req = httpMock.expectOne(`${environment.apiUrl}/servers`);
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual(newServer);
    req.flush(createdServer);
  });

  it('should update a server', () => {
    const updatedServer: Server = {
      id: 1,
      name: 'Updated Server',
      url: 'https://updated.com',
      status: 'Online',
      uptimePercentage: 98,
      responseTimeMs: 120,
      createdAt: new Date(),
      updatedAt: new Date()
    };

    service.updateServer(1, updatedServer).subscribe();

    const req = httpMock.expectOne(`${environment.apiUrl}/servers/1`);
    expect(req.request.method).toBe('PUT');
    expect(req.request.body).toEqual(updatedServer);
    req.flush(null);
  });

  it('should delete a server', () => {
    service.deleteServer(1).subscribe();

    const req = httpMock.expectOne(`${environment.apiUrl}/servers/1`);
    expect(req.request.method).toBe('DELETE');
    req.flush(null);
  });

  it('should get server stats', () => {
    const mockStats = {
      Total: 10,
      Online: 8,
      Offline: 1,
      Degraded: 1
    };

    service.getStats().subscribe(stats => {
      expect(stats).toEqual(mockStats);
    });

    const req = httpMock.expectOne(`${environment.apiUrl}/servers/stats`);
    expect(req.request.method).toBe('GET');
    req.flush(mockStats);
  });
});
