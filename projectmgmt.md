# .NET Framework 4.8 to .NET 10 Modernization Program

## Program Metadata

| **Field** | **Value** |
|-----------|-----------|
| **Title** | .NET Framework 4.8 to .NET 10 Modernization |
| **Objective** | Modernize ContosoUniversity application from .NET Framework 4.8 to .NET 10 with Azure-native services |
| **Execution Model** | Single long-running autonomous execution |
| **Cloud Platform** | Microsoft Azure |
| **Target Region** | southeastasia |
| **Target Framework** | .NET 10 |
| **Start Date** | 2026-01-21 |
| **Status** | In Progress - Phase 1: Discovery and Planning |

## Guiding Principles

1. ✅ Strict SDLC phase gating without exception
2. ✅ All work executed and tracked in projectmgmt.md
3. ✅ No untracked or ad-hoc work permitted
4. ✅ Azure-native, cloud-first services
5. ✅ Automation-first (CI/CD, IaC)
6. ✅ Security by design (Zero Trust, Managed Identity)
7. ✅ Mandatory observability and traceability

## Architecture Mapping

| **Legacy Component** | **Modern Replacement** | **Status** |
|---------------------|----------------------|------------|
| .NET Framework 4.8 | .NET 10 | Pending |
| ASP.NET MVC 5 | ASP.NET Core MVC | Pending |
| IIS | Azure App Service | Pending |
| MSMQ | Azure Service Bus | Pending |
| Local File System | Azure Blob Storage | Pending |
| Web.config | appsettings.json + App Configuration | Pending |
| Secrets in config | Managed Identity + Key Vault | Pending |
| SQL Server LocalDB | Azure SQL Database | Pending |
| Custom logging | Application Insights | Pending |
| Manual deployments | Azure DevOps CI/CD | Pending |
| Manual infra | Bicep or Terraform | Pending |

---

## SDLC Phase 1: Discovery and Planning

### Phase Status: **In Progress**

### Epic 1.1: Architecture and Dependency Analysis
**Status:** In Progress  
**Priority:** Critical  
**Acceptance Criteria:**
- Complete analysis of solution structure
- Identify all Windows-specific dependencies
- Document all third-party libraries
- Assess cloud migration readiness

#### Task 1.1.1: Analyze Solution Structure
**Status:** In Progress  
**Assigned To:** Automation  
**Description:** Analyze the ContosoUniversity solution to understand project structure, dependencies, and build configuration.

**Findings:**
- **Project Type:** ASP.NET MVC 5 Web Application
- **Project Format:** Traditional .csproj (not SDK-style)
- **Target Framework:** .NET Framework 4.8
- **Build Tool:** MSBuild
- **Solution Structure:**
  - ContosoUniversity.csproj (main web application)
  - ContosoUniversity.sln (solution file)

**Dependencies Identified:**
- Microsoft.AspNet.Mvc 5.2.9
- Microsoft.AspNet.WebPages 3.2.9
- Microsoft.AspNet.Razor 3.2.9
- Entity Framework Core 3.1.32
- Microsoft.Data.SqlClient 2.1.4
- Newtonsoft.Json 13.0.3
- System.Messaging (for MSMQ)

**Key Components:**
- Controllers: HomeController, StudentsController, CoursesController, InstructorsController, DepartmentsController, NotificationsController
- Services: NotificationService (MSMQ-based), LoggingService
- Data: SchoolContext (EF Core), DbInitializer
- Models: Student, Course, Instructor, Department, Enrollment, Notification

**Commits:** [Pending]

#### Task 1.1.2: Identify MSMQ Dependencies
**Status:** In Progress  
**Assigned To:** Automation  
**Description:** Identify all MSMQ (Microsoft Message Queuing) usage in the application.

**Findings:**
- **MSMQ Usage:** NotificationService.cs uses System.Messaging
- **Queue Path:** `.\Private$\ContosoUniversityNotifications` (configured in Web.config)
- **Functionality:** Real-time admin notifications for entity CRUD operations
- **Message Format:** JSON-serialized notification objects
- **Integration Points:**
  - BaseController.SendEntityNotification()
  - NotificationsController for retrieving messages
  - Frontend JavaScript polling every 5 seconds

**Migration Strategy:**
- Replace System.Messaging with Azure.Messaging.ServiceBus
- Migrate queue to Azure Service Bus Queue or Topic
- Update configuration to use Azure Service Bus connection string
- Implement Managed Identity for authentication

**Commits:** [Pending]

#### Task 1.1.3: Identify IIS and Windows-Only APIs
**Status:** In Progress  
**Assigned To:** Automation  
**Description:** Identify IIS-specific configurations and Windows-only API usage.

**Findings:**
- **IIS Configuration:**
  - Uses IIS Express for development (configured in .csproj)
  - HTTPS binding on port 44300
  - Windows Authentication enabled
  - Anonymous Authentication disabled
- **Web.config Settings:**
  - System.Web configuration
  - httpRuntime settings
  - requestFiltering configuration
- **Windows-Specific:**
  - System.Messaging (MSMQ) - Windows-only
  - Integrated Security for SQL Server connection
  - File system access for uploads (`Uploads/TeachingMaterials/`)

**Migration Strategy:**
- Remove IIS-specific configuration
- Convert to Kestrel web server
- Replace Windows Authentication with Azure AD authentication
- Replace Integrated Security with Azure SQL Managed Identity
- Replace file system with Azure Blob Storage

**Commits:** [Pending]

#### Task 1.1.4: Assess Entity Framework Compatibility
**Status:** In Progress  
**Assigned To:** Automation  
**Description:** Assess Entity Framework Core version and compatibility with .NET 10.

**Findings:**
- **Current Version:** Entity Framework Core 3.1.32
- **Target Version:** Entity Framework Core 9.x (latest for .NET 10)
- **Database Provider:** Microsoft.EntityFrameworkCore.SqlServer
- **Context:** SchoolContext
- **Initialization:** DbInitializer with seed data
- **Models:** Student, Course, Instructor, Department, Enrollment, CourseAssignment, OfficeAssignment
- **Compatibility Issues:**
  - EF Core 3.1 is outdated and not optimal for .NET 10
  - Need to upgrade to EF Core 9.x
  - Connection string uses LocalDB - needs migration to Azure SQL

**Migration Strategy:**
- Upgrade to Entity Framework Core 9.x
- Update all EF Core NuGet packages
- Test migrations and database compatibility
- Update connection string for Azure SQL Database
- Implement retry policies for cloud resilience

**Commits:** [Pending]

#### Task 1.1.5: Configuration and Secrets Management Assessment
**Status:** In Progress  
**Assigned To:** Automation  
**Description:** Identify configuration files and secrets management approach.

**Findings:**
- **Configuration Files:**
  - Web.config (main configuration)
  - Web.Debug.config (debug transformation)
  - Web.Release.config (release transformation)
- **Configuration Sections:**
  - connectionStrings
  - appSettings
  - system.web
  - system.webServer
- **Secrets Identified:**
  - SQL Server connection string (currently uses Integrated Security)
  - NotificationQueuePath (not a secret but environment-specific)
- **Environment-Specific Values:**
  - Database connection strings
  - Queue paths
  - File upload paths

**Migration Strategy:**
- Convert Web.config to appsettings.json
- Use appsettings.Development.json and appsettings.Production.json
- Store secrets in Azure Key Vault
- Use Managed Identity for authentication to Azure services
- Implement Azure App Configuration for environment-specific settings
- Use User Secrets for local development

**Commits:** [Pending]

#### Task 1.1.6: Third-Party Libraries Assessment
**Status:** In Progress  
**Assigned To:** Automation  
**Description:** Assess all third-party libraries and NuGet packages for .NET 10 compatibility.

**Findings:**
- **Compatible Libraries (need version updates):**
  - Newtonsoft.Json 13.0.3 (compatible but prefer System.Text.Json)
  - Entity Framework Core 3.1.32 → 9.x
  - Microsoft.Data.SqlClient 2.1.4 → latest
- **Framework-Specific Libraries (need replacement):**
  - Microsoft.AspNet.Mvc 5.2.9 → Microsoft.AspNetCore.Mvc
  - Microsoft.AspNet.WebPages → Not needed in ASP.NET Core
  - Microsoft.AspNet.Razor → Microsoft.AspNetCore.Razor
  - System.Web.* → Microsoft.AspNetCore.*
  - System.Messaging → Azure.Messaging.ServiceBus
- **Build/Compilation:**
  - Microsoft.CodeDom.Providers.DotNetCompilerPlatform → Not needed (Roslyn is built-in)
  - WebGrease → Not needed (ASP.NET Core has built-in bundling)
  - Microsoft.AspNet.Web.Optimization → Replace with ASP.NET Core bundling

**Migration Strategy:**
- Replace all .NET Framework-specific packages with .NET Core equivalents
- Update all compatible packages to latest stable versions
- Remove packages that are no longer needed
- Evaluate System.Text.Json vs Newtonsoft.Json

**Commits:** [Pending]

### Epic 1.2: .NET 10 Compatibility Assessment
**Status:** Pending  
**Priority:** Critical  
**Acceptance Criteria:**
- All components assessed for .NET 10 compatibility
- Migration strategy documented
- Risks identified and mitigation planned

**Tasks:** [To be defined after Epic 1.1 completion]

### Epic 1.3: Legacy Component Inventory
**Status:** Pending  
**Priority:** High  
**Acceptance Criteria:**
- Complete inventory of all legacy components
- Replacement strategy for each component
- No unidentified components remain

**Tasks:** [To be defined after Epic 1.1 completion]

### Phase 1 Gate Criteria
- [ ] All work items validated and approved
- [ ] No open or unlinked Tasks or Issues in projectmgmt.md
- [ ] All findings documented and traceable
- [ ] All Tasks linked to Epics

---

## SDLC Phase 2: Design and Modernization
**Status:** Pending  
**Gate:** Blocked by Phase 1 completion

### Epic 2.1: SDK-Style Project Conversion
**Status:** Pending

### Epic 2.2: .NET 10 Upgrade
**Status:** Pending

### Epic 2.3: Legacy API Replacement
**Status:** Pending

### Epic 2.4: MSMQ to Azure Service Bus Migration
**Status:** Pending

### Epic 2.5: Managed Identity and Key Vault Implementation
**Status:** Pending

### Phase 2 Gate Criteria
- [ ] All code compiles on .NET 10
- [ ] No legacy technologies referenced
- [ ] No secrets in source or config
- [ ] All Tasks completed and verified

---

## SDLC Phase 3: Build and Validation
**Status:** Pending  
**Gate:** Blocked by Phase 2 completion

### Epic 3.1: CI Pipeline Creation
**Status:** Pending

### Epic 3.2: Unit and Integration Testing
**Status:** Pending

### Phase 3 Gate Criteria
- [ ] CI pipeline green
- [ ] No failing or skipped tests
- [ ] Build and test validation complete

---

## SDLC Phase 4: Infrastructure and CI/CD
**Status:** Pending  
**Gate:** Blocked by Phase 3 completion

### Epic 4.1: Azure Infrastructure as Code
**Status:** Pending

### Epic 4.2: CI/CD Pipeline Automation
**Status:** Pending

### Phase 4 Gate Criteria
- [ ] Infrastructure deploys idempotently
- [ ] No manual configuration detected
- [ ] Automated deployment validated

---

## SDLC Phase 5: Deployment and Observability
**Status:** Pending  
**Gate:** Blocked by Phase 4 completion

### Epic 5.1: Production Deployment
**Status:** Pending

### Epic 5.2: Observability Enablement
**Status:** Pending

### Phase 5 Gate Criteria
- [ ] Health checks passing
- [ ] Logs, metrics, and traces available
- [ ] Application fully observable

---

## SDLC Phase 6: Iterative Stabilization
**Status:** Pending  
**Gate:** Blocked by Phase 5 completion

### Issues
- No issues created yet

### Phase 6 Gate Criteria
- [ ] Zero open critical or high-severity Issues
- [ ] All fixes linked to builds and deployments
- [ ] No recurring critical Issues

---

## SDLC Phase 7: Post-Deployment Review
**Status:** Pending  
**Gate:** Blocked by Phase 6 completion

### Phase 7 Gate Criteria
- [ ] No unresolved security findings
- [ ] Performance and reliability targets met
- [ ] Cost baseline approved
- [ ] All review actions closed

---

## SDLC Phase 8: Completion
**Status:** Pending  
**Gate:** Blocked by Phase 7 completion

### Phase 8 Gate Criteria
- [ ] No open work items of any type
- [ ] Production stability sustained
- [ ] Program formally closed

---

## Commit and Build Tracking

### Commits
| Date | SHA | Message | Linked Work Items |
|------|-----|---------|-------------------|
| 2026-01-21 | be98083 | Initial plan | Epic 1.1 |

### Build Pipelines
| Date | Build ID | Status | Linked Work Items |
|------|----------|--------|-------------------|
| - | - | - | - |

### Deployments
| Date | Environment | Status | Build ID | Linked Work Items |
|------|-------------|--------|----------|-------------------|
| - | - | - | - | - |

---

## Risk Register

| Risk ID | Description | Probability | Impact | Mitigation | Status |
|---------|-------------|-------------|--------|------------|--------|
| R-001 | MSMQ to Service Bus migration may impact notification delivery | Medium | High | Implement retry logic and dead-letter queue handling | Open |
| R-002 | EF Core 3.1 to 9.x upgrade may break existing queries | Medium | Medium | Thorough testing and query validation | Open |
| R-003 | Windows Authentication replacement may impact user access | Low | High | Implement Azure AD with proper role mapping | Open |
| R-004 | File system to Blob Storage migration may impact performance | Low | Medium | Implement caching and CDN | Open |

---

## Final Outcome Checklist

- [ ] Azure-hosted
- [ ] Secure by default (Zero Trust, Managed Identity)
- [ ] Observable and scalable (Application Insights)
- [ ] Cost-optimized
- [ ] Fully traceable SDLC execution documented in projectmgmt.md
- [ ] All 8 SDLC phases completed
- [ ] No open work items
- [ ] Production stability validated

---

## Notes and Decisions

### 2026-01-21
- **Decision:** Use Azure Service Bus (not Storage Queue) for MSMQ replacement due to richer feature set and better compatibility with existing notification pattern
- **Decision:** Upgrade to EF Core 9.x instead of EF Core 8 to leverage latest .NET 10 features
- **Decision:** Use Bicep for Infrastructure as Code due to better Azure native support
- **Decision:** Prefer System.Text.Json over Newtonsoft.Json for new code, but maintain Newtonsoft.Json for compatibility where needed

---

*Last Updated: 2026-01-21 13:31:36 UTC*
*Program Status: Phase 1 - Discovery and Planning - In Progress*
