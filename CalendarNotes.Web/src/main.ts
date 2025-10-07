import { createApp } from 'vue'
import { createPinia } from 'pinia'
import router from './router'
import App from './App.vue'

// Naive UI стили
import 'vfonts/Lato.css'
import 'vfonts/FiraCode.css'

const app = createApp(App)
const pinia = createPinia()

// Важно: Pinia должна быть установлена ДО router
app.use(pinia)
app.use(router)

app.mount('#app')
