##Description
ExtractZip consists of a pipeline used to extract a .zip file.

##ExtractZip component
The ExtractZip component extracts .zip file entries to one file per entry. 

_No parameters_

Extended behaviour for this component will be added at a later point.

##UnzipMultipart component
Intended to be used for handling xml files with attachments such as pdfs or images where we want to keep them together as one message in BizTalk.
The UnzipMultipart component extracts the zip file entries to one message with a messagepart for each file in the zip file. Body part will default to the first xml file.

_No parameters_

Extended behaviour for this component will be added at a later point.
