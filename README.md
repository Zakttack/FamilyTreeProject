## Family Tree Project
# Table of Contents
1. Overview
2. Motivation & Background
3. Architecture Summary
4. Key Features
5. Technical Focus & Learning
6. Branch Overview
7. Teamwork & Communication
8. Outcome
# Overview
This project modernizes how my family reunion committee maintains our multi‑generational genealogy records. Historically, the family tree was updated manually in a Word document and exported as a PDF. After reviewing the printed book at a reunion, I proposed and began developing a digital platform that automates lineage updates using full‑stack engineering principles, graph theory, and NoSQL document modeling.

The system is built using TypeScript React, .NET 8 Web API, MongoDB, and iText7 PDF parsing, with lineage relationships represented as graph structures stored in flexible NoSQL documents.
# Motivation & Background
This project is both a volunteer contribution and a long-term engineering sandbox, blending personal passion with technical growth. It reinforces my engineering philosophy: clarity, structure, mathematics, and maintainability.
# Architecture Summary
Frontend: TypeScript React  
Backend: .NET 8 Web API  
Database: MongoDB  
PDF Parsing: iText7  
Data Model: Graph Theory lineage representation
# Key Features
- Interactive React UI for exploring hierarchical family structures
- CRUD workflows for reporting marriages, children, and deceased members
- PDF ingestion pipeline using iText7 to parse the original genealogy book
- Graph‑based traversal logic for multi‑branch lineage relationships
- Modular backend services for people, relationships, and lineage metadata
- NoSQL document modeling for flexible, schema‑light storage
# Technical Focus & Learning
## Version 1 — `master` Branch (Stable Release)

Version 1 represents the foundation of the platform and focuses on strengthening core full-stack engineering fundamentals:

- **.NET Backend Development**
  Extended my knowledge of ASP.NET Core, Web API design, layered architecture, and structured service/DAO patterns.

- **Graph Theory Application**
  Modeled family lineage using graph-based relationships, enabling traversal logic for multi-branch family structures.

- **NoSQL Fundamentals (MongoDB)**
  Gained insight into document-based NoSQL storage and schema-flexible modeling for representing people, families, and lineage metadata.

- **Frontend UI Development (React + TypeScript + CSS)**
  Built an interactive React interface for exploring hierarchical family structures and performing CRUD workflows.

- **Open-Source PDF Parsing (iText7)**
  Implemented a PDF ingestion pipeline to parse the original genealogy book and convert legacy data into structured digital records.

Version 1 is fully functional locally and serves as the stable baseline for the project.

## Version 2 — `dev` Branch (Active Development)

The `dev` branch represents ongoing development and expands the system into cloud, DevOps, and advanced architectural concepts. This branch is where I explore modern engineering practices and long-term platform evolution:

- **Azure Cloud Computing**
  Getting hands-on with Azure services, inspired by my foundation from the Doosan Bobcat Co-op. This includes App Configuration, Key Vault, Blob Storage, and future hosting workflows.

- **Prompt Engineering**
  Showcasing early adoption of AI-assisted development using Copilot and Claude as reasoning tools for architecture, debugging, and accelerated learning.

- **Graph Databases (Neo4j)**
  Experimenting with native graph storage to enhance lineage traversal and relationship modeling beyond document-based NoSQL.

- **Caching (Redis)**
  Introducing Redis for global state management across the project. Since both backend and frontend scopes are stateless, caching provides a mechanism for promoting specific states into stateful, persistent representations.

- **Unit & Integration Testing**
  Building habits around breaking problems into smaller components and verifying correctness. This version helps me take testing more seriously and integrate it into my development workflow.

- **Authentication & Authorization**
  Implementing secure access controls so family members can have accounts and permissions aligned with the cloud principle of least privilege.

- **Messaging & Event-Driven Patterns**
  Designing message flows between microservices to support authentication/authorization pipelines — especially mapping legitimate Entra-verified users to Person documents.

- **Docker Containerization**
  Containerizing the platform to support cloud deployment, local reproducibility, and DevOps workflows.

- **Layered Polyglot Persistence**
  Refining storage by combining multiple specialized databases:
  - **Neo4j** for parent-child lineage relationships
  - **Cosmos DB (NoSQL API)** for person documents and family dynamics

- **MVC Architecture Enhancements**
  Refining backend structure with clearer separation of concerns, improved maintainability, and scalable design patterns.

Version 2 is the long-term evolution of the platform and reflects my growth toward becoming a cloud-centric software engineer.