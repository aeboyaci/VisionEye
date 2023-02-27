FROM node:16.15-alpine
WORKDIR /app

ARG NODE_ENVIRONMENT
ENV NODE_ENVIRONMENT=$NODE_ENVIRONMENT

ARG DB_URL
ENV DB_URL=$DB_URL

ARG COOKIE_SECRET
ENV COOKIE_SECRET=$COOKIE_SECRET

ARG GOOGLE_CLIENT_ID
ENV GOOGLE_CLIENT_ID=$GOOGLE_CLIENT_ID

ARG GOOGLE_CLIENT_SECRET
ENV GOOGLE_CLIENT_SECRET=$GOOGLE_CLIENT_SECRET

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

CMD ["npm", "start"]
