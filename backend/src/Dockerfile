FROM node:16.15-alpine
WORKDIR /app

# Copy package.json and package-lock.json
COPY package*.json ./

# Install dependencies
RUN npm install

# Copy source files
COPY . ./

# Generate Prisma.Js Client
RUN npx prisma generate

# Build API
RUN npm run build

EXPOSE 8080

RUN ["npm", "start"]
