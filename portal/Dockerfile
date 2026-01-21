## --------------------------------
## Build Application
## --------------------------------
FROM node:20-alpine AS builder

# Enable Corepack to use pnpm
RUN corepack enable && corepack prepare pnpm@latest --activate

WORKDIR /app

COPY pnpm-lock.yaml package.json ./

RUN pnpm install --frozen-lockfile

COPY . .

RUN pnpm run build

RUN pnpm prune --prod

## --------------------------------
## Run Application
## --------------------------------
FROM node:20-alpine
WORKDIR /app

COPY --from=builder /app/build ./build
COPY --from=builder /app/node_modules ./node_modules
COPY package.json .

ENV NODE_ENV=production
EXPOSE 3000

CMD ["node", "build"]