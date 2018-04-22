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
- All operations are Execute Multiple using 2 threads with batchsize set
- Mapping entities depending the operation type.
- Return GUID on Create operations and al rows processed
- Force specific culture
- Map to specific entity type for lookups.
- Mapping Refresh
- Ouputs: 
        CRM OK with all outputs without errors. 
        CRMErrors with all error outputs.

Supported Operations. 

- Create
- Update
- Delete
- Upsert
- Status
- Execute Workflow



Check Wiki for more details

