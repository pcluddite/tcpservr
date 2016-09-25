# tcpservr
Remote desktop control client/server

This was originally an incredibly powerful application, but as more features got added the project's original code base became unmanageable. Within the past two years, I've undertaken an effort to rewrite the majority of this application's code, and spinning off the most monstrous portion into its own project [TCPSERVR-BASIC](https://github.com/pcluddite/tbasic).

Historically, this project was built as a small portable application to easily set up and transport. All components were included internally. Now, this project is built around its child, tbasic, and the application is nothing more than a remote wrapper for it.

Features include:
- Send and execute scripts on a remote machine
- A shell for connecting and sending single functions to the the server
- Data is sent and transmitted using JSON instead of the original proprietary binary message

A complete manual for TCPSERVR 1.5 is located at tcpservr/manual.docx, but only portions apply to this version.
