# WarehouseScan Application

## Overview
WarehouseScan is a Windows Forms application developed for OOgarden for document scanning and management within warehouse operations. It integrates with multiple internal web services, including the main Sage ERP system, to digitize and organize various document types.

## Key Features
- **Automatic Document Detection**: Identifies document types based on input reference patterns
- **Scanner Integration**: Supports TWAIN-compatible scanners
- **Sage Integration**: Communicates with Sage ERP for document registration
- **Multiple Document Types**: Handles delivery tours, supplier documents, incidents, and ERP orders
- **PDF Generation**: Converts scanned images to organized PDFs
- **Configurable Storage**: Path management based on document types and warehouse codes

## Supported Documents
- Delivery Tour (Loading/Shipping)
- Supplier (Receipts/Deliveries/Waybills)
- Incident Management
- ERP Orders (WEB, NOR, ESC, DEP, SAV, LVC, RTC)
- Oonet Orders

## Integration
- **Sage Web Service**: Primary ERP system for document validation
- **OOgarden Internal Services**: Domain-specific web services for warehouse operations
- **File System**: Organized storage structure with configurable path.
