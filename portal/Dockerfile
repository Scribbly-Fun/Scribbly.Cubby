## --------------------------------
## Build Application
## --------------------------------
FROM node:20-alpine AS builder

# Pin pnpm; @latest tracks breaking majors (ERR_PNPM_IGNORED_BUILDS).
RUN corepack enable && corepack prepare pnpm@10.33.3 --activate

WORKDIR /app

# Workspace config must exist before install so esbuild's build is allowed.
COPY pnpm-lock.yaml package.json pnpm-workspace.yaml ./

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
# adapter-node defaults reconstructed URLs to https; Aspire and other reverse
# proxies forward the browser scheme/host so form actions and redirects match.
ENV PROTOCOL_HEADER=x-forwarded-proto
ENV HOST_HEADER=x-forwarded-host
EXPOSE 3000

CMD ["node", "build"]
