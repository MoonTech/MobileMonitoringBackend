# Mobile Monitoring App Backend Server

Welcome to the backend server of our Mobile Monitoring App for Android! This repository contains a Docker Compose setup that includes the database, transmission server, recording server and API, all seamlessly integrated for easy deployment.

## About project

### Description

The following project concerns the development of the system for a simple use of mobile phones serving as monitoring. The system consists of three modules - mobile application for Android, web application available in the form of a deployed website and a middleware server mediating between these two modules.

The basic aggregate, grouping camera stream from several cameras within its scope, is called room. Mobile application primarily serves the purpose to transmit stream from the camera to the room. The web application is used for room management and stream reception. The functionalities such as creating new rooms, accepting cameras or video recording are implemented in the web application. The middleware server module is used for communication between the mobile application and the web application. It consists of four components: a database, srs service used for image transmission, the rtmp-hls service used for stream recording and a management service (in the form of a REST API). The middleware server enables interaction with database and mediates communication with the srs and rtmp-hls services.

### Examples

Mobile application serves mainly to stream video from your camera to middleware server. Different options to choose rooms, register cameras are allowed.

| Stream Video  | Create rooms  |
| ------------- | ------------- |
| <img src="https://github.com/MoonTech/MobileMonitoringBackend/assets/93740269/a7d503e1-d111-412c-81cd-bf379fa1c4cf" width="300px"/> | <img src="https://github.com/MoonTech/MobileMonitoringBackend/assets/93740269/033e34d7-c47d-4b58-b2cc-63347d53b34c" width="300px"/> |

Web application has the aim of watching multi-view streams, recording videos, accepting cameras and managing rooms. You can also create QR codes which allow mobile devices to join rooms. Web application can serve up to 6 cameras per room with a delay of about 5 seconds.

|    |    |
|----|----|
| <img src="https://github.com/MoonTech/MobileMonitoringBackend/assets/93740269/cb96153d-6ac6-424e-a6d9-995f4eae7e3c" width="600px"/> | <img src="https://github.com/MoonTech/MobileMonitoringBackend/assets/93740269/961396e9-2ad0-4b4d-9ce9-e1ff84094853" width="600px"/> |
| <img src="https://github.com/MoonTech/MobileMonitoringBackend/assets/93740269/e299b3f8-a186-49db-8dcb-71cdb1f5dc4a" width="600px"/> | <img src="https://github.com/MoonTech/MobileMonitoringBackend/assets/93740269/99700980-d7a5-416d-b4aa-f1ccd2f2d956" width="600px"/> |

## Mobile Monitoring Backend

### About this repository

This repository includes Mobile Monitoring Backend created by Jakub Winiarski. C# Web API is integrated with SRS service for streaming videos with short delay and rtmp-hls service for recording and storing videos. Whole Application is written according to REST API standards, using Microsoft Entity Framework for creating user accounts, rooms, recordings and cameras.

<img src="https://github.com/MoonTech/MobileMonitoringBackend/assets/93740269/54d868f1-4940-442f-a935-ec1f89adc0ed" width="400px"/>

Moreover, teh API acts as a middleware between backend and video services. Mobile Monitoring Backend uses the scheme shown below. The codebase has been designed following n-tier layer architecture pattern.

<img src="https://github.com/MoonTech/MobileMonitoringBackend/assets/93740269/5a4d99fe-a500-42fb-b8bf-593e833d08c8" width="400px"/>

### Tests

Integration and unit tests for this projects have been created. They covered both API endpoints calling with custom input data and controller/services functions.

<img src="https://github.com/MoonTech/MobileMonitoringBackend/assets/93740269/4dd4c1d9-02df-4297-af6d-e320ea2b2173" width="300px"/>

## Starting project locally

### Prerequisites

Before you begin, ensure you have installed Docker on your system:

### Docker Compose Services

#### 1. Database

We use SqlServer as our database server. The service is defined in `docker-compose.yml`.

#### 2. Transmission Server

The Transmission Server is responsible for handling data transmission between the mobile devices and the backend. It's configured in srs.conf.

#### 3. Recording Server

Our Recording Server manages and stores video recordings. Configuration file is nginx.conf and dockerfile can be found in rtmp-hls server.

#### 4. API

The API service provides endpoints for the mobile app to interact with the backend (including video server).

### Usage

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
