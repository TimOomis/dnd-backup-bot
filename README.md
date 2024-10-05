# Character Sheet Backup 

## Context
This project was created as a small hobby project after a small discussion within the D&D group I am part of. 

We use [D&D Beyond](https://www.dndbeyond.com/) during our sessions to track the characters' status and items, which happened to be offline for maintenance during a session.

This hampered us a bit. We found that the D&D Beyond site offers PDF exports of these sheets which are semi up-to-date, which suffices for our monthly sessions.

This application functions as a Discord Bot which, given a command (or scheduled event at some point), will download the sheets based on some data about the characters and publish it in a Discord channel.

This is **very intentionally** scoped to _just_ our D&D party, which will be reflected in the code.

## Coding Decisions 

Even though this is a hobby project that might as well have been done in a single file, I tend to use hobby projects as an opportunity to take (what I consider) good practices into account and/or try out new concepts.

This application is created to be in a somewhat "productionizable" state. I am aware most things applied here are overkill otherwise 😁.

Concepts I'm applying
- Hexagonal architecture (also known as ports and adapters architecture)
- Automated tests
- Use of OpenTelemetry (using the free [Grafana](https://grafana.com/) tier).
- CI/CD
- Use of [Strongly typed identifiers](https://en.wikipedia.org/wiki/Strongly_typed_identifier). 
- Handling of transient errors using [Polly](https://github.com/App-vNext/Polly).

Got any pointers or suggestions, feel free to leave them!