# Global Clock / World Time Dashboard

**Project Requirements Document (PRD)**  
**Platform:** C# .NET Console Application  
**Console UI:** Spectre.Console  
**Project:** MSSA Mini-Project  
**Version:** 1.0 - Console MVP

---

## 1. Purpose and Project Goal

### Purpose
Build a console-based world clock that allows a user to view and compare current times across multiple global locations while demonstrating core C# concepts.

### Project Goal
Deliver a complete, interactive console application within the expected 8-12 hour scope. The first version should demonstrate clear problem solving, readable C# structure, and successful completion over unnecessary features.

---

## 2. Requirements

The application is aligned with the following guidelines:

- A Console App is an acceptable framework.
- The application should be interactive and useful as an everyday application.
- Using mock or in-memory data source is preferred; a database is optional and beyond scope for the main implementation.
- The project will showcase branching, loops, methods, classes, OOP design, and data-structure use.
- The project should remain within approximately 8-12 hours of coding effort.

### Project Summary

| Item | Decision |
|---|---|
| Framework | C# .NET Console Application |
| Console UI | Spectre.Console |
| Application Type | Interactive world clock and time-comparison utility |
| Primary Data Structure | `List<ClockLocation>` |
| Time Conversion | .NET `TimeZoneInfo` |
| Data Source | Predefined in-memory location/time-zone data |
| Database / External API | Not required |
| Target Effort | Approximately 8-12 hours of coding effort |

---

## 3. Product Scope

### 3.1 Minimum Viable Product (MVP)

The MVP will:

- Display a dashboard containing the user's currently selected clock locations.
- Display the current local date and time for each selected location.
- Allow the user to add a location from a predefined list of supported locations.
- Allow the user to remove a location from the dashboard.
- Allow the user to compare two selected locations.
- Show the time difference between two compared locations in a clear form.
- Indicate when compared locations are on different calendar days.
- Validate menu and location selections so invalid input does not terminate the program.
- Continue running until the user intentionally chooses to exit.

### 3.2 Explicitly Out of Scope for Version 1

The following are not required for the Console MVP:

- Database persistence or user accounts.
- Weather, currency, flight, mapping, or other external APIs.
- Graphical desktop or mobile UI.
- Web hosting or Azure deployment.
- Authentication, cloud storage, or distributed services.
- Automatic location detection.

---

## 4. Functional Requirements

| ID | Requirement | Description | Priority |
|---|---|---|---|
| FR-01 | Application Menu | The application shall present an interactive main menu from which the user can access the available clock functions or exit. | Must |
| FR-02 | Clock Dashboard | The application shall display all currently selected locations and each location's current local date and time. | Must |
| FR-03 | Add Location | The application shall allow the user to select and add a supported location that is not already on the dashboard. | Must |
| FR-04 | Remove Location | The application shall allow the user to remove a selected location from the dashboard. | Must |
| FR-05 | Compare Locations | The application shall allow the user to select two locations and compare their current local times. | Must |
| FR-06 | Date Difference | The comparison result shall identify when the selected locations are on different calendar dates. | Must |
| FR-07 | Input Validation | The application shall reject invalid menu or location selections and allow the user to try again rather than terminating unexpectedly. | Must |
| FR-08 | Continuous Operation | The application shall return to the appropriate menu after an operation and continue until the user selects Exit. | Must |

---

## 5. Technical and Design Requirements

- Use C# and .NET as the application platform.
- Use Spectre.Console for the console presentation layer, such as menus, prompts, tables, panels, and styled output.
- Use .NET `TimeZoneInfo` for time-zone conversion rather than manually hard-coding UTC offsets.
- Use `List<ClockLocation>` as the primary collection for active dashboard locations.
- Represent a location with a `ClockLocation` class containing the location name and the time-zone identifier needed for conversion.
- Keep clock/time-zone calculation logic separate from console-specific input/output where practical.
- Use methods to divide the program into clear responsibilities instead of placing all logic directly in `Program.cs`.
- Do not use Spectre.Console as a substitute for the required core C# concepts; branching, loops, methods, classes, OOP, and data-structure operations should remain visible in the application logic.

---

## 6. Planned Program Structure

| Component | Responsibility |
|---|---|
| `Program.cs` | Application startup, main control flow, and coordination of menu actions. |
| `ClockLocation.cs` | Model representing a supported city/location and its time-zone identifier. |
| `WorldClockService.cs` | Time retrieval, time-zone conversion, and time-comparison logic. |
| `LocationManager.cs` | Management of selected locations and add/remove/search operations. May be combined with another class if the design becomes unnecessarily complex. |
| Spectre.Console UI code | Presentation of menus, prompts, tables, panels, validation messages, and formatted results. |

The exact class breakdown may be simplified during implementation if a separate class would add complexity without adding useful separation of responsibilities.

---

## 7. Required C# Concepts Demonstrated

| Concept | How the Project Demonstrates It |
|---|---|
| Branching | Menu decisions, validation, add/remove conditions, comparison results, and calendar-day checks. |
| Loops | Main application loop and iteration through collections of locations. |
| Methods | Separate application tasks into named methods with focused responsibilities. |
| Classes / OOP | Use objects to model clock locations and encapsulate related application behavior. |
| Data Structures | Use a generic `List<ClockLocation>` collection to store and manage active locations. |

---

## 8. Console UI Requirements (Spectre.Console)

- Use a clear application title/header and consistent console layout.
- Present primary navigation as an interactive selection menu rather than relying only on raw numbered `Console.WriteLine` prompts.
- Display the world-clock dashboard in a readable table or equivalent structured view.
- Use prompts or selection controls for adding, removing, and comparing locations.
- Use styling only to improve readability and hierarchy; visual polish must not obscure the underlying application logic.
- Keep the interface understandable even if the user has not seen the source code.

---

## 9. MVP Acceptance Criteria

| ID | Acceptance Criterion |
|---|---|
| AC-01 | The program starts successfully and displays the Global Clock interface. |
| AC-02 | The dashboard can show multiple selected locations and their current local date/time. |
| AC-03 | A user can add at least one supported location through the application UI. |
| AC-04 | A user can remove a selected location without terminating the application. |
| AC-05 | A user can select two locations and receive a correct time comparison. |
| AC-06 | The program identifies when compared locations fall on different calendar dates. |
| AC-07 | Invalid selections are handled gracefully and the user can continue. |
| AC-08 | The application continues until the user intentionally exits. |
| AC-09 | The source demonstrates branching, loops, methods, classes/OOP, and a data structure. |
| AC-10 | The console UI uses Spectre.Console while core application logic remains separated enough to support later UI migration. |

---

## 10. Stretch Goals

Stretch goals are considered only after the MVP is complete and stable.

- Display each location's UTC offset.
- Classify a local time as business hours, early morning, evening, or late night.
- Allow the user to enter a proposed meeting time and convert it for another selected location.
- Add minor Spectre.Console polish such as panels or clearer status indicators, provided it does not expand the project beyond the time limit.

---

## 11. Future Expansion After the Console Version

The console application should be structured so its core model and clock/time-zone logic can later be reused behind another interface.

Potential post-project progression:

1. Complete the Console MVP with Spectre.Console.
2. Build either:
   - a .NET MAUI desktop/mobile UI using the same core application logic, or
   - a Blazor web UI using the same core application logic.
3. Optionally deploy a future web version to Azure.
4. Only after the UI conversion is working, consider persistence, user preferences, or additional services if they provide real value.

---

## 12. Non-Functional Requirements

- **Readability:** Names, methods, and class responsibilities should be understandable to another student or instructor reviewing the project.
- **Maintainability:** Major logic should not be unnecessarily coupled to Spectre.Console-specific display code.
- **Reliability:** Common invalid selections should be handled without an unhandled exception or forced restart.
- **Scope control:** MVP functionality takes priority over extra features and visual effects.
- **Explainability:** The developer should be able to explain how locations are stored, how time conversion works, and how the primary menu flow operates.

---

## 13. Definition of Done

Version 1 is complete when:

- All MVP functional requirements marked **Must** are implemented.
- All MVP acceptance criteria pass during normal use.
- The application demonstrates the required C# concepts from the assignment.
- Spectre.Console provides a clear, usable console interface.
- The program handles expected invalid selections without crashing.
- No database, external API, cloud service, or additional framework is required for the core application to function.
- Stretch goals are not required for project completion.
