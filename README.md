# 🚀 Tool2Go – A Modular Tool Rental System (C#/.NET 8)
**Enterprise-style CLI application with XML persistence, booking logic, validation rules, and a clean service-oriented architecture.**

Tool2Go is a fully functional **tool rental management system** developed in **C#/.NET 8**, focusing on clean architecture, robust input handling, domain-driven design, and real-world business logic.  
It simulates the full lifecycle of renting professional tools — including customers, bookings, categories, availability logic, validation, and cost calculations.

This project demonstrates:

- 🧩 **Deep understanding of domain modeling**
- 🛠️ **Clean service-layer architecture**
- 📦 **XML-based persistence** (serialization/deserialization)
- 🧹 **Input parsing, validation & error handling**
- 🔄 **Complex booking workflows with date validation**
- 🔎 **Debugging & edge-case handling** (temporary reservations, availability checks, etc.)
- ♻️ **Extendable design for future UI or DB upgrades**

---

## 🧱 Architecture Overview

The system follows a **layered architecture** designed for maintainability:

```text
Program.cs (Entry point / Menu control)
├── Services
│   ├── CustomerService
│   ├── BookingService
│   ├── ToolService
│   └── CategoryService
├── Models
│   ├── Customer
│   ├── Booking / BookingPos
│   ├── ToolType / ToolInstance
│   └── ToolCategory
└── Persistence
    └── XML Serialization (Utils)
```

### 🔍 Highlights
- **Services contain all business logic** → no logic in `Program.cs`
- **Models contain only state + simple calculations**
- **Centralized InputHelper** ensures all input is validated & abortable
- **XMLSerializer abstraction** allows future upgrade to JSON/SQL

---

## 🧩 Key Features

### ✔ Smart Booking Workflow
The booking system guides the user step-by-step:

1. Select customer  
2. Enter start & end date  
3. System checks availability for each tool type  
4. User can book multiple tools at once  
5. Age restrictions enforced (21+ for insured tools)  
6. Temporary reservations prevent double-booking in same session  
7. Full summary screen before saving  

---

### ✔ Temporary Reservations (Advanced Feature)
To prevent inconsistent availability:

- tools selected during the booking session are **temporarily reserved**
- ensures correct availability calculation
- prevents multi-adding the same tool type incorrectly

---

### ✔ Robust Input Handling (Industrial-grade)
Every input uses the **InputHelper**, providing:

- Centralized parsing  
- Abort options at any point  
- Retry on invalid input  
- “Press Enter to keep previous value” when editing  
- Consistency across all services  

This simulates real enterprise CLI tooling where input reliability is critical.

---

### ✔ Cancelable Operations
All user-facing actions support:

- Cancel before saving  
- Confirmation before deletion  
- Clear error prompts  
- Retry loops without throwing exceptions

---

### ✔ XML Persistence
All data is stored using XML serialization:

- Tools  
- Tool instances  
- Categories  
- Customers  
- Bookings  
- Nested booking positions  

Ensured through:

- a consistent schema  
- safe loading  
- future-proof structure for migrations 
