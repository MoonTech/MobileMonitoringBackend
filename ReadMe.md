# Mobile Monitoring App Backend Server

Welcome to the backend server of our Mobile Monitoring App for Android! This repository contains a Docker Compose setup that includes the database, transmission server, recording server and API, all seamlessly integrated for easy deployment.

## Prerequisites

Before you begin, ensure you have installed Docker on your system:

## Docker Compose Services

### 1. Database

We use SqlServer as our database server. The service is defined in `docker-compose.yml`.

### 2. Transmission Server

The Transmission Server is responsible for handling data transmission between the mobile devices and the backend. It's configured in srs.conf.

### 3. Recording Server

Our Recording Server manages and stores video recordings. Configuration file is nginx.conf and dockerfile can be found in rtmp-hls server.

### 4. API

The API service provides endpoints for the mobile app to interact with the backend (including video server).

## Usage

To start the entire backend stack, run the following command in the project root:

```bash
docker-compose up -d --build
```

To stop the services:

```bash
docker-compose down
```

## License

This project is created for Bachelors degree at Faculty of Mathematics and Information Sciences of Warsaw University of Technology. All rights of WUT degrees apply.
