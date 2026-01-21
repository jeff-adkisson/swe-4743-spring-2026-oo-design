FROM eclipse-temurin:21-jdk

WORKDIR /app

COPY src/main/java ./src/main/java

RUN find src/main/java -name "*.java" > sources.txt \
    && javac -d out @sources.txt

ENTRYPOINT ["java", "-cp", "out", "edu.kennesaw.crazy8s.Main"]
