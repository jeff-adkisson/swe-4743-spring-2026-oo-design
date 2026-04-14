# Build stage
FROM node:20-alpine AS build
WORKDIR /src

COPY package.json package-lock.json ./
RUN npm ci

COPY . ./
RUN npx ng build --configuration=production

# Runtime stage — static files served by nginx
FROM nginx:alpine AS runtime

# Angular's production build output lives under dist/client/browser
COPY --from=build /src/dist/client/browser /usr/share/nginx/html
COPY nginx.conf /etc/nginx/conf.d/default.conf

EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
