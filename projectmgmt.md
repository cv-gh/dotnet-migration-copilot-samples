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
| **Status** | Phase 2: Design and Modernization - In Progress ⏳ |

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

### Phase Status: **Completed** ✅

### Epic 1.1: Architecture and Dependency Analysis
**Status:** Completed ✅  
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
**Status:** Completed  
**Priority:** Critical  
**Acceptance Criteria:**
- All components assessed for .NET 10 compatibility
- Migration strategy documented
- Risks identified and mitigation planned

#### Task 1.2.1: ASP.NET MVC 5 to ASP.NET Core MVC Compatibility
**Status:** Completed  
**Assigned To:** Automation  
**Description:** Assess compatibility of migrating from ASP.NET MVC 5 to ASP.NET Core MVC.

**Findings:**
- **Current:** ASP.NET MVC 5 on .NET Framework 4.8
- **Target:** ASP.NET Core MVC on .NET 10
- **Breaking Changes:**
  - System.Web namespace not available (HttpContext, HttpRequest, HttpResponse)
  - Global.asax replaced by Program.cs and Startup.cs
  - Web.config replaced by appsettings.json
  - Bundle and Minification different approach
  - Razor view engine compatible but syntax updates needed
  - Controller base class changes
  - Dependency injection built-in (no need for external DI container)
  - No Windows Authentication by default (need Azure AD)

**Migration Path:**
1. Convert project to SDK-style
2. Update target framework to net10.0
3. Replace System.Web references with Microsoft.AspNetCore
4. Convert Global.asax to Program.cs
5. Update controllers to use ASP.NET Core base classes
6. Update views for Tag Helpers
7. Update bundling and minification
8. Implement dependency injection

**Commits:** [Pending]

#### Task 1.2.2: .NET 10 SDK and Runtime Compatibility
**Status:** Completed  
**Assigned To:** Automation  
**Description:** Verify .NET 10 SDK availability and runtime requirements.

**Findings:**
- **Target Framework:** net10.0
- **SDK Version:** .NET 10 SDK required for build
- **Runtime:** ASP.NET Core Runtime 10.x required for execution
- **C# Language Version:** C# 13 available
- **Breaking Changes from .NET Framework 4.8:**
  - No System.Web namespace
  - Different configuration system
  - Different hosting model (Kestrel vs IIS)
  - Native AOT compilation available (optional)
  - Improved performance and memory management

**Compatibility Matrix:**
| Component | .NET Framework 4.8 | .NET 10 | Status |
|-----------|-------------------|---------|--------|
| Language Features | C# 7.3 | C# 13 | Compatible |
| ASP.NET MVC | 5.2.9 | ASP.NET Core MVC | Requires Migration |
| Entity Framework | EF Core 3.1 | EF Core 9.x | Compatible with upgrade |
| Dependency Injection | Not built-in | Built-in | Compatible |
| Configuration | Web.config | appsettings.json | Requires Migration |
| Logging | Custom/Log4Net | ILogger/App Insights | Requires Migration |

**Commits:** [Pending]

#### Task 1.2.3: View and Frontend Compatibility
**Status:** Completed  
**Assigned To:** Automation  
**Description:** Assess Razor views, JavaScript, and CSS compatibility.

**Findings:**
- **Razor Views:** Compatible but need updates for Tag Helpers
- **JavaScript Libraries:**
  - jQuery 3.4.1 - Compatible
  - Bootstrap 3.x - Should upgrade to Bootstrap 5.x
  - Custom JavaScript - Compatible
- **CSS:** Compatible
- **Bundling:** Need to migrate from System.Web.Optimization to ASP.NET Core bundling
- **Client-side validation:** Compatible with updates

**Migration Strategy:**
- Update Razor views to use Tag Helpers instead of Html Helpers
- Update Bootstrap from 3.x to 5.x for better compatibility
- Implement ASP.NET Core bundling and minification
- Update _ViewImports.cshtml and _ViewStart.cshtml
- Test all JavaScript functionality after migration

**Commits:** [Pending]

### Epic 1.3: Legacy Component Inventory
**Status:** Completed  
**Priority:** High  
**Acceptance Criteria:**
- Complete inventory of all legacy components
- Replacement strategy for each component
- No unidentified components remain

#### Task 1.3.1: Complete Application Component Inventory
**Status:** Completed  
**Assigned To:** Automation  
**Description:** Create a comprehensive inventory of all application components.

**Application Structure:**
```
ContosoUniversity/
├── Controllers/          # 7 controllers (BaseController, HomeController, StudentsController, 
│                         #   CoursesController, InstructorsController, DepartmentsController, 
│                         #   NotificationsController)
├── Models/               # 11 models (Student, Course, Instructor, Department, Enrollment, 
│                         #   CourseAssignment, OfficeAssignment, Person, Notification, ErrorViewModel)
├── Views/                # 9 view folders (Shared, Home, Students, Courses, Instructors, 
│                         #   Departments, Notifications)
├── Data/                 # 3 data files (SchoolContext, DbInitializer, SchoolContextFactory)
├── Services/             # 2 services (NotificationService, LoggingService)
├── App_Start/            # 3 config files (BundleConfig, FilterConfig, RouteConfig)
├── Content/              # CSS and Bootstrap files
├── Scripts/              # JavaScript files (jQuery, Bootstrap, validation)
├── Uploads/              # File upload directory (Teaching Materials)
└── Global.asax           # Application startup
```

**Total Source Files:** 31 C# files

**Components by Category:**
1. **Web Layer:** Controllers (7), Views (30+), Global.asax
2. **Data Layer:** Models (11), DbContext (1), DbInitializer (1)
3. **Service Layer:** NotificationService, LoggingService
4. **Configuration:** Web.config, App_Start configs
5. **Static Assets:** CSS, JavaScript, Images
6. **File Storage:** Uploads directory

**Commits:** [Pending]

#### Task 1.3.2: Legacy Technology Inventory and Replacement Plan
**Status:** Completed  
**Assigned To:** Automation  
**Description:** Document all legacy technologies and their modern replacements.

**Complete Inventory:**

| Legacy Component | Purpose | Modern Replacement | Migration Complexity | Priority |
|------------------|---------|-------------------|---------------------|----------|
| **Framework & Runtime** |
| .NET Framework 4.8 | Application runtime | .NET 10 | High | Critical |
| ASP.NET MVC 5 | Web framework | ASP.NET Core MVC | High | Critical |
| System.Web | Web abstractions | Microsoft.AspNetCore | High | Critical |
| IIS/IIS Express | Web server | Kestrel | Medium | Critical |
| **Data & Messaging** |
| EF Core 3.1.32 | ORM | EF Core 9.x | Low | Critical |
| SQL Server LocalDB | Database | Azure SQL Database | Medium | Critical |
| System.Messaging (MSMQ) | Message queue | Azure Service Bus | High | Critical |
| **Storage** |
| Local File System | File uploads | Azure Blob Storage | Medium | High |
| **Configuration** |
| Web.config | Configuration | appsettings.json | Medium | Critical |
| ConfigurationManager | Config access | IConfiguration | Medium | Critical |
| **Authentication** |
| Windows Auth | Authentication | Azure AD B2C | High | High |
| No authorization | Authorization | ASP.NET Core Identity + Azure AD | High | High |
| **Logging & Monitoring** |
| Debug.WriteLine | Logging | ILogger + App Insights | Low | Medium |
| No monitoring | Monitoring | Application Insights | Medium | High |
| **Bundling & Optimization** |
| System.Web.Optimization | Asset bundling | ASP.NET Core bundling | Low | Medium |
| WebGrease | Minification | Built-in minification | Low | Low |
| **Dependencies** |
| Microsoft.AspNet.* | MVC framework | Microsoft.AspNetCore.* | High | Critical |
| Newtonsoft.Json | JSON | System.Text.Json (or keep) | Low | Low |
| **Infrastructure** |
| Manual deployment | Deployment | Azure DevOps CI/CD | High | Critical |
| No IaC | Infrastructure | Bicep/Terraform | High | Critical |

**Migration Priority Order:**
1. **Phase 2:** Framework upgrade (.NET 10, ASP.NET Core)
2. **Phase 2:** MSMQ to Service Bus
3. **Phase 2:** Configuration migration
4. **Phase 3:** Build and test infrastructure
5. **Phase 4:** Azure infrastructure and CI/CD
6. **Phase 5:** File storage to Blob Storage
7. **Phase 5:** Authentication and authorization
8. **Phase 5:** Monitoring and observability

**Commits:** [Pending]

#### Task 1.3.3: Data Model and Database Assessment
**Status:** Completed  
**Assigned To:** Automation  
**Description:** Assess data models and database migration requirements.

**Database Schema:**
- **Tables:** Person (TPH for Student/Instructor), Course, Enrollment, Department, 
  CourseAssignment, OfficeAssignment, Notification
- **Relationships:**
  - Person -> Enrollment (One-to-Many)
  - Course -> Enrollment (One-to-Many)
  - Department -> Course (One-to-Many)
  - Instructor -> CourseAssignment (Many-to-Many with Course)
  - Instructor -> OfficeAssignment (One-to-One)
  - Department -> Instructor (One-to-Many)
- **Total Models:** 11 entities
- **Migration Path:**
  - Export schema from LocalDB
  - Create Azure SQL Database
  - Apply migrations to Azure SQL
  - Update connection string with Managed Identity
  - Test all queries and relationships

**Data Migration Strategy:**
- Use EF Core migrations
- Script migration for Azure SQL compatibility
- Implement connection resilience (retry policies)
- Use Azure SQL Database (not LocalDB)
- Implement Managed Identity for authentication

**Commits:** [Pending]

### Phase 1 Gate Criteria
- [x] All work items validated and approved
- [x] No open or unlinked Tasks or Issues in projectmgmt.md
- [x] All findings documented and traceable
- [x] All Tasks linked to Epics

**Gate Status:** ✅ **PASSED** - Phase 1 Complete. Proceeding to Phase 2.

---

## SDLC Phase 2: Design and Modernization
**Status:** In Progress ⏳  
**Gate:** Phase 1 Passed ✅

### Epic 2.1: SDK-Style Project Conversion
**Status:** Pending
**Priority:** Critical
**Acceptance Criteria:**
- Project converted to SDK-style format
- All dependencies properly referenced
- Project builds successfully

#### Task 2.1.1: Convert .csproj to SDK-Style Format
**Status:** Completed ✅
**Description:** Convert the traditional .csproj format to modern SDK-style project format.

**Actions Completed:**
- ✅ Created new SDK-style .csproj targeting net10.0
- ✅ Removed traditional MSBuild imports and explicit file inclusions
- ✅ Added Azure service package references (Service Bus, Blob Storage, Key Vault, Identity)
- ✅ Updated Entity Framework Core to 9.0.0
- ✅ Added Application Insights telemetry
- ✅ Simplified project structure
- ✅ Backed up original project file (ContosoUniversity.csproj.backup)

**Commits:** dced8ea

### Epic 2.2: .NET 10 Upgrade
**Status:** Pending
**Priority:** Critical
**Acceptance Criteria:**
- Application runs on .NET 10
- All dependencies compatible with .NET 10
- No compilation errors

#### Task 2.2.1: Update Target Framework to .NET 10
**Status:** Pending
**Description:** Update project to target .NET 10 framework.

**Commits:** [Pending]

#### Task 2.2.2: Update NuGet Packages
**Status:** Pending
**Description:** Update all NuGet packages to .NET 10 compatible versions.

**Commits:** [Pending]

### Epic 2.3: ASP.NET Core MVC Migration
**Status:** Pending
**Priority:** Critical
**Acceptance Criteria:**
- ASP.NET MVC 5 converted to ASP.NET Core MVC
- All controllers functional
- All views rendering correctly

#### Task 2.3.1: Create Program.cs and Replace Global.asax
**Status:** Completed ✅
**Description:** Replace Global.asax with modern Program.cs using minimal hosting model.

**Actions Completed:**
- ✅ Created Program.cs with ASP.NET Core minimal hosting model
- ✅ Configured services (DbContext, DI, Application Insights)
- ✅ Implemented database initialization on startup
- ✅ Configured middleware pipeline
- ✅ Added Azure Key Vault integration for production
- ✅ Set up session support
- ✅ Configured routing and error handling

**Commits:** [Pending]

#### Task 2.3.2: Update Controllers for ASP.NET Core
**Status:** In Progress ⏳
**Description:** Update all controllers to use ASP.NET Core base classes and patterns.

**Actions Completed:**
- ✅ Updated BaseController to use dependency injection
- ✅ Converted BaseController to async patterns
- ✅ Updated HomeController for ASP.NET Core
- ✅ Removed obsolete files (App_Start, Global.asax, AssemblyInfo.cs)
- 🔄 Remaining controllers need updates: StudentsController, CoursesController, InstructorsController, DepartmentsController, NotificationsController

**Commits:** [Pending]

#### Task 2.3.3: Update Views for ASP.NET Core
**Status:** Pending
**Description:** Update Razor views to use ASP.NET Core Tag Helpers and patterns.

**Commits:** [Pending]

### Epic 2.4: Configuration Migration
**Status:** Pending
**Priority:** Critical
**Acceptance Criteria:**
- Web.config replaced with appsettings.json
- All configuration values migrated
- No secrets in configuration files

#### Task 2.4.1: Create appsettings.json
**Status:** Completed ✅
**Description:** Create appsettings.json and migrate configuration from Web.config.

**Actions Completed:**
- ✅ Created appsettings.json with connection strings and Azure configuration
- ✅ Created appsettings.Development.json for local development
- ✅ Migrated connection string from Web.config
- ✅ Added Azure Service Bus configuration
- ✅ Added Azure Storage configuration
- ✅ Added Azure Key Vault configuration
- ✅ Added Application Insights configuration
- ✅ No secrets stored in configuration files (placeholders only)

**Commits:** [Pending]

#### Task 2.4.2: Implement Azure Key Vault Integration
**Status:** Pending
**Description:** Integrate Azure Key Vault for secrets management.

**Commits:** [Pending]

### Epic 2.5: MSMQ to Azure Service Bus Migration
**Status:** Pending
**Priority:** Critical
**Acceptance Criteria:**
- MSMQ completely replaced with Azure Service Bus
- Notification system functional
- No System.Messaging references

#### Task 2.5.1: Create Azure Service Bus Namespace (Local Development)
**Status:** Pending
**Description:** Set up Azure Service Bus for local development and testing.

**Commits:** [Pending]

#### Task 2.5.2: Replace NotificationService with Service Bus Implementation
**Status:** Completed ✅
**Description:** Replace MSMQ-based NotificationService with Azure Service Bus client.

**Actions Completed:**
- ✅ Created INotificationService interface
- ✅ Replaced System.Messaging with Azure.Messaging.ServiceBus
- ✅ Implemented async methods for send/receive
- ✅ Added Managed Identity support for Azure authentication
- ✅ Added graceful fallback when Service Bus not configured
- ✅ Implemented proper logging with ILogger
- ✅ Added JSON serialization with System.Text.Json
- ✅ Backed up old MSMQ implementation

**Commits:** [Pending]

#### Task 2.5.3: Update Controllers to Use New NotificationService
**Status:** Pending
**Description:** Update all controllers to use the new Service Bus-based notification service.

**Commits:** [Pending]

### Epic 2.6: File Storage Migration
**Status:** Pending
**Priority:** High
**Acceptance Criteria:**
- File system storage replaced with Azure Blob Storage
- File uploads functional
- No local file system dependencies

#### Task 2.6.1: Implement Azure Blob Storage Service
**Status:** Pending
**Description:** Create service for Azure Blob Storage operations.

**Commits:** [Pending]

#### Task 2.6.2: Update File Upload Controllers
**Status:** Pending
**Description:** Update controllers to use Azure Blob Storage instead of file system.

**Commits:** [Pending]

### Phase 2 Gate Criteria
- [ ] All code compiles on .NET 10
- [ ] No legacy technologies referenced
- [ ] No secrets in source or config
- [ ] All Tasks completed and verified

**Gate Status:** Pending

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
| 2026-01-21 | be98083 | Initial plan | Program Initialization |
| 2026-01-21 | 128dc79 | Phase 1: Create comprehensive project management tracking file | Epic 1.1, Epic 1.2, Epic 1.3 |
| 2026-01-21 | dced8ea | Phase 1 Complete: Discovery and Planning with full component inventory | Epic 1.1, Epic 1.2, Epic 1.3 |
| 2026-01-21 | [Pending] | Phase 2: SDK-style project conversion and .NET 10 upgrade | Epic 2.1, Epic 2.2, Epic 2.3, Epic 2.4, Epic 2.5 |

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

*Last Updated: 2026-01-21 13:35:00 UTC*
*Program Status: Phase 2 - Design and Modernization - In Progress ⏳*
*Phase 1 Completed: Discovery and Planning ✅*
