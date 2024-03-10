<script setup lang="ts">

import {nextTick, reactive, ref} from "vue";
import Code from "./components/Code.vue";
import { FontSizeOutlined, FolderOpenOutlined,PictureOutlined  } from '@ant-design/icons-vue';
const data = reactive<
   { list: Array<PasteInfo> }
>({
  list: []
})
declare var window: {
  getData: Function,
  chrome: {
    webview: {
      hostObjects: {
        bridge: {
          getList: (page: number, pageSize: number) => Promise<string>,
          handleSelect: (id: number) => void,
          handleCopy: (id: number) => void,
        }
      }
    }
  }
}
const bridge = window.chrome.webview.hostObjects.bridge;

interface PasteInfo {
  Id: number,
  Title: string,
  Content: String,
  Image: any,
  Type: "Text" | "Bitmap" | "FileDrop" ,
  TimeStr: string
}
async function handleClick() {
  console.log('click', bridge)
  const res =await bridge.getList(0, 100)
  const lists: Array<PasteInfo> = JSON.parse(res)
  console.log(lists)
  data.list = lists
  if (curr.value === 0) curr.value = lists[0].Id
}
window.getData = () => handleClick()

handleClick()
const curr = ref(0)
document.querySelector("body")?.addEventListener("keydown", async function(event) {

  if (event.key === "ArrowUp") {
    curr.value ++
  }
  if (event.key === "ArrowDown") {
    curr.value --
  }
  if (event.key  === 'Enter') {
    await bridge.handleSelect(curr.value)
  }

  console.log(event.key)
  nextTick(() => {
    if (document?.scrollingElement && document?.scrollingElement?.scrollTop) {
      const currDom = document.querySelector(".item_"+curr.value) as HTMLElement
      (document?.scrollingElement as HTMLElement).scrollTop = currDom.offsetTop - currDom.offsetHeight - 40 || 0
    }
  })
});

async function copy(id: number) {
  await bridge.handleCopy(id)
}

</script>


<template>
  <div id="app" class="bg-white p-1 pr-[20px]">
    <div class="flex flex-col gap-2">
      <div v-for="(item, _) in data.list" :key="item.Id"
           @click="curr = item.Id"
        :class="`${'item_'+item.Id} ${curr === item.Id ? 'border-blue-500' : ''} rounded-xl cursor-pointer  shadow border-2 border-amber-200 p-2 text-left`">
        <div class="flex text-[12px] justify-between mb-[10px] items-center ">
          <div class=" flex items-center">
            <span class="mr-1 -mt-1.5">
              <FontSizeOutlined v-if="item.Type === 'Text'" />
              <PictureOutlined v-if="item.Type === 'Bitmap'"/>
              <FolderOpenOutlined v-if="item.Type === 'FileDrop'"/>
            </span>
            <span class="text-black">{{ item.TimeStr}}</span>
          </div>
          <div>
            <span class="text-blue-500" @click="copy(item.Id)">复制</span>
          </div>
        </div>
        <div v-if="item.Type === 'Text'" class=" break-all">
          <Code :code="String(item.Title || '').trim()"/>
        </div>
        <div v-if="item.Type === 'Bitmap'" class="text-red-300">
          <img class="mt-1 max-h-[200px] m-auto" :src="`data:image/png;base64,${item.Image}`" alt="">
        </div>
        <div v-if="item.Type === 'FileDrop'" class="text-black break-all">
          [文件] <div>{{ item.Content }}</div>
        </div>
      </div>
    </div>
  </div>

</template>

