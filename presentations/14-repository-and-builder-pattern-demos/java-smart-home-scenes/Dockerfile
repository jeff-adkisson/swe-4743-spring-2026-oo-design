FROM maven:3.9.11-eclipse-temurin-17 AS build
WORKDIR /src

COPY pom.xml ./
RUN mvn -q dependency:go-offline

COPY src ./src
RUN mvn -q -DskipTests package

FROM eclipse-temurin:17-jre
WORKDIR /app

COPY --from=build /src/target/lecture14-java-scenes.jar ./lecture14-java-scenes.jar

ENV SCENE_DB_PATH=/data/smart-home-scenes.db
VOLUME ["/data"]

ENTRYPOINT ["java", "-jar", "/app/lecture14-java-scenes.jar"]
