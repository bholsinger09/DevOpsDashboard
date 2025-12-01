# Test-Driven Development (TDD) Guide

## Overview

This project follows Test-Driven Development practices with comprehensive test coverage for both backend and frontend.

---

## Backend Tests (xUnit + Moq + FluentAssertions)

### Running Tests

```bash
cd backend.tests

# Run all tests
dotnet test

# Run tests with coverage
dotnet test --collect:"XPlat Code Coverage"

# Run specific test
dotnet test --filter "FullyQualifiedName~ServersControllerTests"

# Run tests in watch mode
dotnet watch test
```

### Test Structure

```
backend.tests/
├── Controllers/          # Controller tests
│   ├── ServersControllerTests.cs
│   ├── DeploymentsControllerTests.cs
│   └── LogsControllerTests.cs
├── Services/            # Service tests
│   └── ServerMonitorServiceTests.cs
├── Models/              # Model tests
│   └── ModelTests.cs
└── DevOpsDashboard.Tests.csproj
```

### Test Coverage

- ✅ **Controllers**: Full CRUD operations for all endpoints
- ✅ **Services**: Business logic and background jobs
- ✅ **Models**: Entity validation and defaults
- ✅ **In-Memory Database**: Real database operations without persistence

### Writing Backend Tests

```csharp
[Fact]
public async Task MethodName_ExpectedBehavior_WhenCondition()
{
    // Arrange - Setup test data
    var server = new Server { Name = "Test", Url = "https://test.com" };
    
    // Act - Execute the method
    var result = await _controller.CreateServer(server);
    
    // Assert - Verify the outcome
    result.Should().NotBeNull();
    result.Value.Name.Should().Be("Test");
}
```

---

## Frontend Tests (Jasmine + Karma)

### Running Tests

```bash
cd frontend

# Run tests once
npm test

# Run tests in watch mode
npm test -- --watch

# Run tests with coverage
npm test -- --code-coverage

# Run specific test file
npm test -- --include='**/server.service.spec.ts'
```

### Test Structure

```
frontend/src/app/
├── services/
│   ├── server.service.spec.ts
│   └── deployment.service.spec.ts
└── pages/
    ├── dashboard/
    │   └── dashboard.page.spec.ts
    └── servers/
        └── servers.page.spec.ts
```

### Test Coverage

- ✅ **Services**: HTTP requests and API integration
- ✅ **Components**: Page initialization and user interactions
- ✅ **UI Logic**: Status colors, filtering, and formatting

### Writing Frontend Tests

```typescript
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

  it('should get all servers', () => {
    const mockServers = [/* test data */];
    
    service.getServers().subscribe(servers => {
      expect(servers).toEqual(mockServers);
    });

    const req = httpMock.expectOne(`${environment.apiUrl}/servers`);
    req.flush(mockServers);
  });
});
```

---

## TDD Workflow

### 1. Write the Test First (Red)

```csharp
[Fact]
public async Task GetServer_ReturnsNotFound_WhenServerDoesNotExist()
{
    // Act
    var result = await _controller.GetServer(999);

    // Assert
    result.Result.Should().BeOfType<NotFoundResult>();
}
```

### 2. Run the Test (Fails)

```bash
dotnet test
# ❌ Test fails - method not implemented
```

### 3. Write Minimal Code (Green)

```csharp
public async Task<ActionResult<Server>> GetServer(int id)
{
    var server = await _context.Servers.FindAsync(id);
    if (server == null)
        return NotFound();
    return server;
}
```

### 4. Run the Test (Passes)

```bash
dotnet test
# ✅ Test passes
```

### 5. Refactor (Clean)

Improve code quality while keeping tests green.

---

## Test Categories

### Unit Tests
- Test individual methods in isolation
- Mock dependencies
- Fast execution
- No external dependencies

### Integration Tests
- Test multiple components together
- Use in-memory database
- Test API endpoints end-to-end

### Component Tests (Frontend)
- Test Angular components
- Test user interactions
- Test data binding

---

## Best Practices

### Backend

1. **Arrange-Act-Assert (AAA) Pattern**
   ```csharp
   // Arrange
   var server = new Server { Name = "Test" };
   
   // Act
   var result = await _controller.CreateServer(server);
   
   // Assert
   result.Should().NotBeNull();
   ```

2. **Use In-Memory Database**
   ```csharp
   var options = new DbContextOptionsBuilder<ApplicationDbContext>()
       .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
       .Options;
   ```

3. **Mock External Dependencies**
   ```csharp
   var mockHub = new Mock<IHubContext<DashboardHub>>();
   ```

4. **Test One Thing Per Test**
   - Each test should verify a single behavior
   - Clear test names describe what's being tested

5. **Clean Up Resources**
   ```csharp
   public void Dispose()
   {
       _context.Database.EnsureDeleted();
       _context.Dispose();
   }
   ```

### Frontend

1. **Use TestBed Configuration**
   ```typescript
   beforeEach(async () => {
     await TestBed.configureTestingModule({
       declarations: [MyComponent],
       imports: [HttpClientTestingModule],
       providers: [MyService]
     }).compileComponents();
   });
   ```

2. **Mock HTTP Requests**
   ```typescript
   const req = httpMock.expectOne(url);
   expect(req.request.method).toBe('GET');
   req.flush(mockData);
   ```

3. **Test Async Operations**
   ```typescript
   it('should load data', (done) => {
     service.getData().subscribe(data => {
       expect(data).toBeTruthy();
       done();
     });
   });
   ```

4. **Spy on Dependencies**
   ```typescript
   const serviceSpy = jasmine.createSpyObj('MyService', ['getData']);
   serviceSpy.getData.and.returnValue(of(mockData));
   ```

---

## Code Coverage Goals

### Backend
- **Target**: 80%+ line coverage
- **Current**: Run `dotnet test --collect:"XPlat Code Coverage"` to check

### Frontend
- **Target**: 70%+ line coverage
- **Current**: Run `npm test -- --code-coverage` to check

### Viewing Coverage Reports

**Backend:**
```bash
cd backend.tests
dotnet test --collect:"XPlat Code Coverage"
# Coverage report in TestResults/*/coverage.cobertura.xml
```

**Frontend:**
```bash
cd frontend
npm test -- --code-coverage
# Open coverage/index.html in browser
```

---

## Continuous Testing

### Watch Mode (Backend)
```bash
cd backend.tests
dotnet watch test
```

### Watch Mode (Frontend)
```bash
cd frontend
npm test -- --watch
```

---

## CI/CD Integration

### GitHub Actions Example

```yaml
name: Tests

on: [push, pull_request]

jobs:
  test-backend:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v2
      - name: Setup .NET
        uses: actions/setup-dotnet@v1
      - name: Run tests
        run: |
          cd backend.tests
          dotnet test --collect:"XPlat Code Coverage"
  
  test-frontend:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v2
      - name: Setup Node
        uses: actions/setup-node@v2
      - name: Install dependencies
        run: cd frontend && npm install
      - name: Run tests
        run: cd frontend && npm test -- --watch=false --code-coverage
```

---

## Common Test Scenarios

### Testing API Endpoints
```csharp
[Fact]
public async Task CreateServer_ReturnsCreatedResult()
{
    var server = new Server { Name = "New", Url = "https://new.com" };
    var result = await _controller.CreateServer(server);
    result.Result.Should().BeOfType<CreatedAtActionResult>();
}
```

### Testing Service Logic
```csharp
[Fact]
public async Task CheckServerAsync_UpdatesLastChecked()
{
    var server = new Server { Name = "Test", Url = "https://google.com" };
    _context.Servers.Add(server);
    await _context.SaveChangesAsync();
    
    await _service.CheckServerAsync(server.Id);
    
    var updated = await _context.Servers.FindAsync(server.Id);
    updated.LastChecked.Should().NotBeNull();
}
```

### Testing HTTP Calls
```typescript
it('should create a server', () => {
  const newServer = { name: 'Test', url: 'https://test.com' };
  
  service.createServer(newServer).subscribe(server => {
    expect(server.id).toBeGreaterThan(0);
  });
  
  const req = httpMock.expectOne(`${apiUrl}/servers`);
  expect(req.request.method).toBe('POST');
  req.flush({ id: 1, ...newServer });
});
```

---

## Troubleshooting

### Backend Test Issues

**In-memory database conflicts:**
```csharp
// Use unique database name per test
var options = new DbContextOptionsBuilder<ApplicationDbContext>()
    .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
    .Options;
```

**Mock setup issues:**
```csharp
// Ensure all required methods are mocked
mockService.Setup(x => x.MethodName()).ReturnsAsync(result);
```

### Frontend Test Issues

**HTTP requests not verified:**
```typescript
afterEach(() => {
  httpMock.verify(); // Ensures all requests were handled
});
```

**Async timing issues:**
```typescript
// Use fakeAsync and tick for timing control
it('should handle delay', fakeAsync(() => {
  service.delayedMethod();
  tick(1000);
  expect(result).toBeTruthy();
}));
```

---

## Resources

- [xUnit Documentation](https://xunit.net/)
- [Moq Documentation](https://github.com/moq/moq4)
- [FluentAssertions Documentation](https://fluentassertions.com/)
- [Jasmine Documentation](https://jasmine.github.io/)
- [Angular Testing Guide](https://angular.io/guide/testing)

---

**Happy Testing! 🧪**
