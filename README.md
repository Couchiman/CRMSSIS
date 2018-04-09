# CRMSSIS
CRM SSIS Integration Components for Dynamics CRM

- Connection Manager 
- Source Adapter (more testing & bugfixing needed)
- Destination Adapter (in progress)

Connection Manager

- Online & Onpremise support
- Connection Timeout
- IFD (onpremise), Active Directory (online & onpremise), Office365 (online)
- Impersonate User
- SSL
- Port
- Username & Password 

Source Adapter

- FetchXML Query

Destination Adapter
- Batch Size
- All operations are Bulk using 2 threads.
- Mapping entities depending the operation type.
- Return GUID on Create operations and al rows processed
- Mapping Refresh
- Ouputs: 
        CRM OK with all outputs without errors. 
        CRMErrors with all error outputs.

Supported Operations. 

- Create
- Update
- Delete
- Upsert



Check Wiki for more details

